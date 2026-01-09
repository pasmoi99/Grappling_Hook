using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static TMPro.TMP_Compatibility;

public class GrapplingGun : MoveAroundPlayer
{
    private Stack<RopeSegment> _ropeStackElements;

    private Rigidbody2D _grapplingGunRB;

    //private DistanceJoint2D _distanceJoint;

    private Aim _target;

    private Transform _ropeContainerTransform;
    
    private GrapplingHook _hook;

    private Vector3 _lastTargetWorldPos;

    private float _ropeSegmentBaseYSize;
    private float _ropeSegmentRealYSize;

    private InstantiateParameters _ropeInstantiateParameters;

    
    [SerializeField] private float _ropeLaunchTimer;

    private bool _isRopePresent;
    private bool _mouse0Held;
    private bool _hasCoroutineSpawnRopeStarted;
    private bool _hasCoroutineDespawnRopeStarted;
    private bool _ropeMustComeBack;


    new protected void Start()
    {
        base.Start();

        _ropeStackElements = new Stack<RopeSegment>();

        _grapplingGunRB = GetComponent<Rigidbody2D>();

        //_distanceJoint = GetComponent<DistanceJoint2D>();

        _target = MainGame.Main.TargetAim;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.tag == "RopeContainer")
            {
                _ropeContainerTransform = obj;
                break;
            }
        }

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.tag == "Hook")
            {
                _hook = obj.GetComponent<GrapplingHook>();
                break;
            }
        }

        //_objectToConnectTo = _hook;

        _lastTargetWorldPos = _target.transform.position;

        _ropeSegmentBaseYSize = MainGame.Main.RopeSprite.bounds.size.y;
        _ropeSegmentRealYSize = _ropeSegmentBaseYSize * MainGame.Main.Rope.transform.localScale.y;

        _ropeInstantiateParameters = new InstantiateParameters();
        _ropeInstantiateParameters.parent = _ropeContainerTransform;
        _ropeInstantiateParameters.worldSpace = false;

        _mouse0Held = false;
        _isRopePresent = false;
        _hasCoroutineSpawnRopeStarted = false;
        _hasCoroutineDespawnRopeStarted = false;
        
        _ropeContainerTransform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _mouse0Held = true;
            LockPosition(true);
        }
        else
        {
            _mouse0Held = false;
        }

        if (GetCanMoveAroundPlayer())
        {
            _lastTargetWorldPos = _target.transform.position;


            Move(_lastTargetWorldPos);
        }
        else
        {

            
            Move(_lastTargetWorldPos);
        }
    }
    private void FixedUpdate()
    {
        if (_mouse0Held && !_isRopePresent && !_hasCoroutineSpawnRopeStarted)
        {
            StartCoroutine(SpawnRope(Vector3.up * _ropeSegmentRealYSize, _target.transform.position,true));
            _hasCoroutineSpawnRopeStarted = true;

        }
        else if (!_mouse0Held && _isRopePresent && !_hasCoroutineDespawnRopeStarted)
        {
            StartCoroutine(DespawnRope());
            _hasCoroutineDespawnRopeStarted = true;
        }
    }
    protected void Move(Vector3 target)
    {
        _grapplingGunRB.MovePosition(_player.transform.position + ((target - _player.transform.position).normalized * p_distanceFromPlayer));
        _grapplingGunRB.MoveRotation( Quaternion.LookRotation(Vector3.forward, (transform.position - _player.transform.position).normalized));
    }
    private IEnumerator SpawnRope(Vector3 currentPos, Vector3 targetPos, bool isStarting, float totalDistance = 0)
    {
        if (totalDistance >= Mathf.Sqrt(
            Mathf.Pow(targetPos.x - transform.position.x, 2) +
            Mathf.Pow(targetPos.y - transform.position.y, 2)) /*|| !_hook.CheckIfAnyWallIsHit()*/)
        {
            _isRopePresent = true;
            _hasCoroutineDespawnRopeStarted = false;
            _hasCoroutineSpawnRopeStarted = false;

            yield break;

        }
        else
        {

            _hook.transform.localPosition += Vector3.up * _ropeSegmentRealYSize;

            RopeSegment segment = Instantiate<RopeSegment>(MainGame.Main.Rope, currentPos - (Vector3.up * _ropeSegmentRealYSize),
                Quaternion.identity, _ropeInstantiateParameters);

            _ropeContainerTransform.localPosition += Vector3.up * _ropeSegmentRealYSize;

            _ropeStackElements.Push(segment);

            //if (isStarting)
            //{
            //    segment.GetComponent<HingeJoint2D>().enabled = true;
            //    Debug.Log(segment.GetComponent<HingeJoint2D>().enabled);
            //    segment.GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
            //    segment.GetComponent<HingeJoint2D>().connectedBody = _hook.GetComponent<Rigidbody2D>();
            //    //segment.ConnectToPreviousGameObject(, _objectToConnectTo);
            //}
            //else
            //{
            //    segment.GetComponent<HingeJoint2D>().enabled = true;
            //    segment.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -_ropeSegmentRealYSize);
            //    segment.GetComponent<HingeJoint2D>().connectedBody = _objectToConnectTo.GetComponent<Rigidbody2D>();
            //}

            currentPos = segment.transform.localPosition;

            totalDistance += _ropeSegmentRealYSize;

            yield return new WaitForSeconds(_ropeLaunchTimer);

            StartCoroutine(SpawnRope(currentPos,targetPos,false, totalDistance));
            yield break;
        }
    }

    private IEnumerator DespawnRope()
    {
        _ropeStackElements.TrimExcess();
        
        int size = _ropeStackElements.Count;

        for (int i = 0;i<size; i++)
        {
            _ropeContainerTransform.localPosition += Vector3.down * _ropeSegmentRealYSize;

            _hook.transform.localPosition += Vector3.down * _ropeSegmentRealYSize;

            Destroy(_ropeStackElements.Peek().gameObject);

            _ropeStackElements.Pop();

            yield return new WaitForSeconds(_ropeLaunchTimer);
        }

        _ropeContainerTransform.localPosition = Vector3.zero;
        
        _isRopePresent=false;

        _hasCoroutineDespawnRopeStarted = false;
        _hasCoroutineSpawnRopeStarted = false;

        LockPosition(false);
        yield break;
    }
}
