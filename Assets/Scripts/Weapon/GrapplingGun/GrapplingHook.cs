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

    private AudioSource _hookSource;

    private void Start()
    {
        _lastHitGameObject = null;
        _lastPosition = transform.position;
        _wall = MainGame.Main.WallMask;
        _hookSource = GetComponent<AudioSource>();
    }

    
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
       
        if (_lastHitGameObject == null)
        {
            
            return false;
        }

        else if (!_lastHitGameObject.TryGetComponent<Wall>(out Wall w) || !w.GetCanClingToThis())
        {
            _hookSource.PlayOneShot(SoundManager.Sounds.HittingClang);
            return false;
        }
        else
        {
            _lastPosition = transform.position;
            _hookSource.PlayOneShot(SoundManager.Sounds.Latching);
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
