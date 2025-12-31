using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MoveAroundPlayer
{

    private Aim _target;

    private Transform _ropeContainerTransform;

    private GrapplingHook _hook;

    private float _ropeSegmentBaseYSize;
    private float _ropeSegmentCurrentYSize;

    private Queue<GameObject> _ropeQueueElements;

    [SerializeField] private float _ropeLaunchTimer;

    private bool _isRopePresent;
    private bool _mouse0Held;
    private bool _hasCoroutineStarted;


    new protected void Start()
    {
        base.Start();
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

        _ropeContainerTransform.localPosition = Vector3.zero;
        
        _ropeSegmentBaseYSize = MainGame.Main.Rope.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        _ropeSegmentCurrentYSize = _ropeSegmentBaseYSize * MainGame.Main.Rope.transform.localScale.y;
        _ropeQueueElements = new Queue<GameObject>();
        _mouse0Held = false;
        _isRopePresent = false;
        _hasCoroutineStarted = false;
    }

    new protected void Update()
    {
        base.Update();
        transform.rotation = Quaternion.LookRotation(Vector3.forward, (_target.transform.position - transform.position).normalized);
        if (Input.GetMouseButton(0))
        {
            _mouse0Held = true;
        }
        else
        {
            _mouse0Held = false;
        }
    }
    private void FixedUpdate()
    {
        if (_mouse0Held && !_isRopePresent && !_hasCoroutineStarted)
        {
            //LockPosition(true);
            StartCoroutine(SpawnRope(transform.position));
            _hasCoroutineStarted = true;

        }
        else if (!_mouse0Held && _isRopePresent && !_hasCoroutineStarted)
        {
            StartCoroutine(DespawnRope());
            _hasCoroutineStarted = true;
        }
    }
    private IEnumerator SpawnRope(Vector3 CurrentPos,float totalDistance=0)
    {
        if (totalDistance >= Mathf.Sqrt(
            Mathf.Pow(_target.transform.position.x - transform.position.x, 2) +
            Mathf.Pow(_target.transform.position.y - transform.position.y, 2)) || _hook.CheckIfCorrectWallIsHit())
        {
            _isRopePresent = true;

            //Debug.Log("number of items spawned: " + (n-1));
            //Debug.Log("totalDistance: " + totalDistance);
            //Debug.Log("_ropeBaseYSize" + _ropeBaseYSize);
            //Debug.Log("_ropeCurrentYSize" + _ropeCurrentYSize);
            //Debug.Log("_ropeBaseYSize * n" + (_ropeBaseYSize*(n-1)));
            //Debug.Log("n * currentsize" + ((n-1)*_ropeCurrentYSize));
            //Debug.Log("dist between gun and target: " + Mathf.Sqrt(
            //Mathf.Pow(_target.transform.position.x - transform.position.x, 2) +
            //Mathf.Pow(_target.transform.position.y - transform.position.y, 2)));

            _hasCoroutineStarted = false;

            yield break;


        }
        else
        {

            Vector3 ropeDirection = (_target.transform.position - CurrentPos).normalized;

            _hook.transform.position += ropeDirection * _ropeSegmentCurrentYSize;

            GameObject obj = Instantiate(MainGame.Main.Rope, CurrentPos + (ropeDirection * _ropeSegmentCurrentYSize),
                Quaternion.LookRotation(Vector3.forward, ropeDirection), _ropeContainerTransform.transform);

            _ropeQueueElements.Enqueue(obj);

            CurrentPos = obj.transform.position;

            totalDistance += _ropeSegmentCurrentYSize;

            yield return new WaitForSeconds(_ropeLaunchTimer);

            StartCoroutine(SpawnRope(CurrentPos,totalDistance));
            yield break;
        }
    }

    private IEnumerator DespawnRope()
    {
        _ropeQueueElements.TrimExcess();
        
        int size = _ropeQueueElements.Count;

        Vector3 ropeDirection = (transform.position - _hook.transform.position).normalized;

        for (int i = 0;i<size; i++)
        {
            _ropeContainerTransform.position += ropeDirection * _ropeSegmentCurrentYSize;

            _hook.transform.position += ropeDirection * _ropeSegmentCurrentYSize;

            Destroy(_ropeQueueElements.Peek());

            _ropeQueueElements.Dequeue();

            yield return new WaitForSeconds(_ropeLaunchTimer);
        }

        _ropeContainerTransform.localPosition = Vector3.zero;
        
        _isRopePresent=false;

        _hasCoroutineStarted = false;

        yield break;
    }
}
