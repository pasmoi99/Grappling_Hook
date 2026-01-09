using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private float _ySizeHook;
    private GameObject _lastHitGameObject;
    private Vector3 _localUp;
    //private float _ySizeRopeSegment;

    private void Start()
    {
        _ySizeHook = GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.localScale.y;
        _lastHitGameObject = null;
        _localUp = transform.InverseTransformPoint(Vector3.up);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, (MainGame.Main.TargetTransform.position - transform.position).normalized * _ySizeHook);
    //}
    public bool CheckIfAnyWallIsHit()
    {
        Vector3 direction = (MainGame.Main.TargetAim.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _ySizeHook);
        if (hit.collider != null)
        {
            _lastHitGameObject = hit.collider.gameObject;
            return true;
        }
        else
        {  
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
            return true;
        }

    }
    

}
