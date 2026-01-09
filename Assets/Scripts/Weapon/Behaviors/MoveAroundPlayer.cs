using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundPlayer : MonoBehaviour
{
    private bool _canMoveAroundPlayer;

    protected Player _player;

    [SerializeField] protected float p_distanceFromPlayer;

    protected void Start()
    {
        _player = MainGame.Main.PlayerChara;
        _canMoveAroundPlayer = true;
    }

    //protected void Update()
    //{
    //    if (_canMoveAroundPlayer)
    //    {
    //        //Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Move();
    //    }

    //}

    protected void LockPosition(bool Lock)
    {
        _canMoveAroundPlayer = !Lock;
    }
    protected bool GetCanMoveAroundPlayer()
    {
        return _canMoveAroundPlayer;
    }
}
