using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MoveAroundPlayer
{
    
    private Aim _target;
    
    private Transform _ropeContainer;

    private GameObject _hook;
    
    private float _ropeBaseYSize;
    private float _ropeCurrentYSize;

    //[SerializeField] private float _ropeLaunchTimer;
    
    private bool _isRopePresent;
    private bool _mouse0Held;

    new protected void Start()
    {
        base.Start();
        _mouse0Held = false;
        _isRopePresent = false;
        _target = MainGame.Main.TargetAim;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if(obj.tag == "RopeContainer")
            {
                _ropeContainer = obj;
                break;
            }
        }

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.tag == "Hook")
            {
                _hook = obj.gameObject;
                break;
            }
        }

        _ropeBaseYSize = MainGame.Main.Rope.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        _ropeCurrentYSize = _ropeBaseYSize * MainGame.Main.Rope.transform.localScale.y;
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
        if (_mouse0Held && !_isRopePresent)
        {
            //LockPosition(true);
            SpawnRope(transform.position);
            
        }
        else
        {

        }
    }
    private void SpawnRope(Vector3 CurrentPos, float totalDistance = 0,int n=1)
    {
        if (totalDistance >= Mathf.Sqrt(
            Mathf.Pow(_target.transform.position.x - transform.position.x, 2) +
            Mathf.Pow(_target.transform.position.y - transform.position.y, 2)))
        {
            _isRopePresent = true;
            Debug.Log("number of items spawned: " + (n-1));
            Debug.Log("totalDistance: " + totalDistance);
            Debug.Log("_ropeBaseYSize" + _ropeBaseYSize);
            Debug.Log("_ropeCurrentYSize" + _ropeCurrentYSize);
            Debug.Log("_ropeBaseYSize * n" + (_ropeBaseYSize*(n-1)));
            Debug.Log("n * currentsize" + ((n-1)*_ropeCurrentYSize));
            Debug.Log("dist between gun and target: " + Mathf.Sqrt(
            Mathf.Pow(_target.transform.position.x - transform.position.x, 2) +
            Mathf.Pow(_target.transform.position.y - transform.position.y, 2)));
            return;
        }
        else 
        {

            Vector3 ropeDirection = (_target.transform.position - CurrentPos).normalized;
            
            GameObject obj = Instantiate(MainGame.Main.Rope, CurrentPos + (ropeDirection * _ropeCurrentYSize),
                Quaternion.LookRotation(Vector3.forward, ropeDirection),_ropeContainer.transform);



            CurrentPos = obj.transform.position;

            totalDistance += _ropeCurrentYSize;
            
            SpawnRope(CurrentPos, totalDistance,n+1);
        }
    }
}
