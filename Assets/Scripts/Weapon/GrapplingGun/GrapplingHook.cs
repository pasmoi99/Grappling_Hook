using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    //private float _ySizeHook;

    private GameObject _lastHitGameObject;

    private Vector3 _lastPosition;

    private LayerMask _wall;

    [SerializeField] private List<Sprite> _smoke;
    //private float _ySizeRopeSegment;

    private void Start()
    {
        //_ySizeHook = GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.localScale.y;
        _lastHitGameObject = null;
        _lastPosition = transform.position;
        _wall = MainGame.Main.WallMask;
        //_localUp = transform.InverseTransformPoint(Vector3.up);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, (MainGame.Main.TargetTransform.position - transform.position).normalized * _ySizeHook);
    //}
    public bool CheckIfAnyWallIsHit(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0,_wall);
        if (hit.collider != null)
        {
            _lastHitGameObject = hit.collider.gameObject;
            return true;
        }
        else
        {  
            _lastHitGameObject = null;
            return false;
        }
    }

    public bool CheckIfCorrectWallIsHit()
    {
       
        if (_lastHitGameObject == null || !_lastHitGameObject.TryGetComponent<Wall>(out Wall w) || !w.GetCanClingToThis())
        {
            return false;
        }
        else
        {
            _lastPosition = transform.position;
            Instantiate(MainGame.Main.HookSmoke,transform.position,Quaternion.identity);
            return true;
        }

    }
    
    public void StopMovement()
    {
        transform.position = _lastPosition;
    }


    public void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }
}
