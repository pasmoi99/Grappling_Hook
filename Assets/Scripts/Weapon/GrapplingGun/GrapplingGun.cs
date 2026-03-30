using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MoveAroundPlayer
{
    private Stack<RopeSegment> _ropeStackElements;

    private DistanceJoint2D _distanceJointChara;

    private Aim _target;

    private Transform _ropeContainerTransform;


    private Vector3 _lastTargetWorldPos;

    private float _ropeSegmentBaseYSize;
    private float _ropeSegmentRealYSize;
    private float _distanceFromHook;

    private InstantiateParameters _ropeInstantiateParameters;

    private bool _isRopePresent;
    private bool _mouse0Held;
    private bool _hasCoroutineSpawnRopeStarted;
    private bool _hasCoroutineDespawnRopeStarted;
    private bool _lockHookPosition;
    private bool _ropeMustComeBack;
    private bool _ropeCanSpawn;
    private bool _canPlayRopeLaunchSound;

    private AudioSource _grapplingGunSource;

    [SerializeField] private float _ropeLaunchTimer;
    [SerializeField] private float _ropeMaxDistance;
    [SerializeField] private GrapplingHook _hook;
    //[SerializeField] private float _distanceRaycastHook;

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

        //foreach (Transform transform in GetComponentsInChildren<Transform>())
        //{
        //    if (transform.tag == "Hook")
        //    {
        //        _hook = transform.GetComponent<GrapplingHook>();
        //        break;
        //    }
        //}

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
        _canPlayRopeLaunchSound = false;

        _ropeContainerTransform.localPosition = Vector3.zero;

        _grapplingGunSource = GetComponent<AudioSource>();
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
            _canPlayRopeLaunchSound = true;
            _isRopePresent = true;
            _hasCoroutineSpawnRopeStarted = true;
            StartCoroutine(SpawnRope(Vector3.right * _ropeSegmentRealYSize, _lastTargetWorldPos));
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
        else if (!_mouse0Held && !_isRopePresent)
        {
            _ropeCanSpawn = true;
            LockPosition(false);
        }
    }

    protected void Move(Vector3 target)
    {
        transform.position = _player.transform.position + ((target - _player.transform.position).normalized * p_distanceFromPlayer);
        transform.right = target - transform.position;
        /*transform.rotation = Quaternion.LookRotation(Vector3.forward, (transform.position - _player.transform.position).normalized)*/;
    }

    private IEnumerator SpawnRope(Vector3 currentPos, Vector3 targetPos, float previousDistance = Mathf.Infinity)
    {
        if (_canPlayRopeLaunchSound)
        {
            _canPlayRopeLaunchSound = false;
            _grapplingGunSource.PlayOneShot(SoundManager.Sounds.RopeFiring);
        }

        float distTemp = Vector3.Distance(targetPos, _hook.transform.position);

        if (!_mouse0Held
            || previousDistance <= distTemp

            || _distanceFromHook >= _ropeMaxDistance)
        {
            Debug.Log("Rope size" + _distanceFromHook);
            Debug.Log("distance between gun and target" + Mathf.Sqrt(
                Mathf.Pow(targetPos.x - transform.position.x, 2) +
                Mathf.Pow(targetPos.y - transform.position.y, 2)));
            Debug.Log("target pos" + _lastTargetWorldPos);
            Debug.Log("hook pos" + _hook.transform.position);
            Debug.Log(_lastTargetWorldPos - _hook.transform.position);

            _ropeMustComeBack = true;

            _hasCoroutineDespawnRopeStarted = false;

            _grapplingGunSource.Stop();

            yield break;
            
        }
        else if (_hook.CheckIfAnyWallIsHit((targetPos - _hook.transform.position).normalized))
        {
            if (_hook.CheckIfCorrectWallIsHit())
            {
                _lockHookPosition = true;
                _grapplingGunSource.Stop();
                _distanceJointChara.distance = Mathf.Sqrt(Mathf.Pow(_hook.transform.position.x - MainGame.Main.PlayerChara.transform.position.x, 2) +
                    Mathf.Pow(_hook.transform.position.y - MainGame.Main.PlayerChara.transform.position.y, 2));
                
                _distanceJointChara.enabled = true;
            }
            else
            {
                _ropeMustComeBack = true;
            }
            _grapplingGunSource.Stop();

            _hasCoroutineDespawnRopeStarted = false;

            yield break;

        }
        else
        {

            _hook.transform.localPosition += Vector3.right * _ropeSegmentRealYSize;

            RopeSegment segment = Instantiate<RopeSegment>(MainGame.Main.Rope, currentPos - (Vector3.right * _ropeSegmentRealYSize),
               Quaternion.Euler(0,0,-90) , _ropeInstantiateParameters);

            _ropeContainerTransform.localPosition += Vector3.right * _ropeSegmentRealYSize;

            _ropeStackElements.Push(segment);

            currentPos = segment.transform.localPosition;

            _distanceJointChara.distance += _ropeSegmentRealYSize;

            _distanceFromHook += _ropeSegmentRealYSize;

            yield return new WaitForSeconds(_ropeLaunchTimer);

            StartCoroutine(SpawnRope(currentPos, targetPos,distTemp));
            yield break;
        }
    }

    private IEnumerator DespawnRope()
    {
        _grapplingGunSource.PlayOneShot(SoundManager.Sounds.RopeRetracting);
        _lockHookPosition = false;

        _distanceJointChara.enabled = false;

        _ropeStackElements.TrimExcess();

        int size = _ropeStackElements.Count;

        for (int i = 0; i < size; i++)
        {
            _ropeContainerTransform.localPosition += Vector3.left * _ropeSegmentRealYSize;

            _hook.transform.localPosition += Vector3.left * _ropeSegmentRealYSize;

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
        _grapplingGunSource.Stop();
        yield break;
    }
}
