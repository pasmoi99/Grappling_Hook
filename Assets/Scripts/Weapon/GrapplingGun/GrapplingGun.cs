using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MoveAroundPlayer
{
    private Stack<RopeSegment> _ropeStackElements;

    private DistanceJoint2D _distanceJointChara;

    private Aim _target;

    private Transform _ropeContainerTransform;

    private GrapplingHook _hook;

    private Vector3 _lastTargetWorldPos;

    private float _ropeSegmentBaseYSize;
    private float _ropeSegmentRealYSize;
    private float _distanceFromHook;

    private InstantiateParameters _ropeInstantiateParameters;

    [SerializeField] private float _ropeLaunchTimer;
    [SerializeField] private float _ropeMaxDistance;

    private bool _isRopePresent;
    private bool _mouse0Held;
    private bool _hasCoroutineSpawnRopeStarted;
    private bool _hasCoroutineDespawnRopeStarted;
    private bool _lockHookPosition;
    private bool _ropeMustComeBack;
    private bool _ropeCanSpawn;


    new protected void Start()
    {
        base.Start();

        _ropeStackElements = new Stack<RopeSegment>();

        _distanceJointChara = MainGame.Main.PlayerChara.GetComponent<DistanceJoint2D>();
        _distanceJointChara.enabled = false;

        _target = MainGame.Main.TargetAim;

        foreach (Transform transform in GetComponentsInChildren<Transform>())
        {
            if (transform.tag == "RopeContainer")
            {
                _ropeContainerTransform = transform;
                break;
            }
        }

        foreach (Transform transform in GetComponentsInChildren<Transform>())
        {
            if (transform.tag == "Hook")
            {
                _hook = transform.GetComponent<GrapplingHook>();
                break;
            }
        }

        _lastTargetWorldPos = _target.transform.position;

        _ropeSegmentBaseYSize = MainGame.Main.RopeSprite.bounds.size.y;
        _ropeSegmentRealYSize = _ropeSegmentBaseYSize * MainGame.Main.Rope.transform.localScale.y;
        _distanceFromHook = 0;

        _ropeInstantiateParameters = new InstantiateParameters();
        _ropeInstantiateParameters.parent = _ropeContainerTransform;
        _ropeInstantiateParameters.worldSpace = false;

        _mouse0Held = false;
        _isRopePresent = false;
        _hasCoroutineSpawnRopeStarted = false;
        _hasCoroutineDespawnRopeStarted = false;
        _lockHookPosition = false;
        _ropeMustComeBack = false;
        _ropeCanSpawn = true;

        _ropeContainerTransform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _mouse0Held = true;

        }
        else
        {
            _lockHookPosition = false;
            _mouse0Held = false;
        }

        if (_lockHookPosition)
        {
            _hook.StopMovement();
        }

        if (GetCanMoveAroundPlayer() && !_isRopePresent)
        {
            _lastTargetWorldPos = _target.transform.position;

            Move(_lastTargetWorldPos);
        }
        else if(!_lockHookPosition)
        {
            Move(_lastTargetWorldPos);
        }
        else if (_lockHookPosition)
        {
            Move(_hook.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (_ropeCanSpawn && _mouse0Held && !_isRopePresent && !_hasCoroutineSpawnRopeStarted)
        {
            _isRopePresent = true;
            _hasCoroutineSpawnRopeStarted = true;
            StartCoroutine(SpawnRope(Vector3.up * _ropeSegmentRealYSize, _lastTargetWorldPos));
            _ropeCanSpawn = false;
            LockPosition(true);
        }

        else if (_ropeMustComeBack && _mouse0Held && _isRopePresent && !_hasCoroutineDespawnRopeStarted)
        {
            _ropeMustComeBack = false;
            _hasCoroutineDespawnRopeStarted = true;
            StartCoroutine(DespawnRope());
            _ropeCanSpawn = false;
            LockPosition(true);
        }

        else if (!_mouse0Held && _isRopePresent && !_hasCoroutineDespawnRopeStarted)
        {
            _hasCoroutineDespawnRopeStarted = true;
            StartCoroutine(DespawnRope());
            _ropeCanSpawn = false;
            LockPosition(true);
            
        }
        else if (!_mouse0Held)
        {
            _ropeCanSpawn = true;
            LockPosition(false);
        }
    }

    protected void Move(Vector3 target)
    {
        transform.position = _player.transform.position + ((target - _player.transform.position).normalized * p_distanceFromPlayer);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, (transform.position - _player.transform.position).normalized);
    }

    private IEnumerator SpawnRope(Vector3 currentPos, Vector3 targetPos)
    {
        if (!_mouse0Held
            ||_distanceFromHook >= Mathf.Sqrt(
            Mathf.Pow(targetPos.x - transform.position.x, 2) +
            Mathf.Pow(targetPos.y - transform.position.y, 2))
            || _hook.CheckIfAnyWallIsHit((targetPos - _hook.transform.position).normalized)
            || _distanceFromHook >= _ropeMaxDistance)
        {

            if (_hook.CheckIfCorrectWallIsHit())
            {
                _lockHookPosition = true;
            }
            else if (!_hook.CheckIfAnyWallIsHit((targetPos - _hook.transform.position).normalized) || !_hook.CheckIfCorrectWallIsHit())
            {
                _ropeMustComeBack = true;
            }

            _distanceJointChara.enabled = true;

            _hasCoroutineDespawnRopeStarted = false;

            yield break;

        }
        else
        {

            _hook.transform.localPosition += Vector3.up * _ropeSegmentRealYSize;

            RopeSegment segment = Instantiate<RopeSegment>(MainGame.Main.Rope, currentPos - (Vector3.up * _ropeSegmentRealYSize),
                Quaternion.identity, _ropeInstantiateParameters);

            _ropeContainerTransform.localPosition += Vector3.up * _ropeSegmentRealYSize;

            _ropeStackElements.Push(segment);

            currentPos = segment.transform.localPosition;

            _distanceJointChara.distance += _ropeSegmentRealYSize;

            _distanceFromHook += _ropeSegmentRealYSize;

            yield return new WaitForSeconds(_ropeLaunchTimer);

            StartCoroutine(SpawnRope(currentPos, targetPos));
            yield break;
        }
    }

    private IEnumerator DespawnRope()
    {

        _lockHookPosition = false;

        _distanceJointChara.enabled = false;

        _ropeStackElements.TrimExcess();

        int size = _ropeStackElements.Count;

        for (int i = 0; i < size; i++)
        {
            _ropeContainerTransform.localPosition += Vector3.down * _ropeSegmentRealYSize;

            _hook.transform.localPosition += Vector3.down * _ropeSegmentRealYSize;

            _distanceFromHook -= _ropeSegmentRealYSize;

            Destroy(_ropeStackElements.Peek().gameObject);

            _ropeStackElements.Pop();

            _distanceJointChara.distance -= _ropeSegmentRealYSize;

            yield return new WaitForSeconds(_ropeLaunchTimer);
        }

        _ropeContainerTransform.localPosition = Vector3.zero;


        _hook.ResetPosition();

        _hasCoroutineSpawnRopeStarted = false;
        _isRopePresent = false;
        yield break;
    }


}
