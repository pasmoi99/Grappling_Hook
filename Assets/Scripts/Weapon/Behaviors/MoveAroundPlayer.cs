using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundPlayer : MonoBehaviour
{
    protected Player _player;
    //protected bool _canMoveAroundPlayer;

    [SerializeField,Range(1,100)] protected float p_percentDistanceFromPlayer;

    protected void Start()
    {
        _player = MainGame.Main.PlayerChara;
        //_canMoveAroundPlayer = true;
    }

    protected void Update()
    {
        //if (_canMoveAroundPlayer)
        //{
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = _player.transform.position + ((mouse - _player.transform.position).normalized * (p_percentDistanceFromPlayer / 100));
        //}

    }

    public void LockPosition(bool Lock)
    {
        //_canMoveAroundPlayer = !Lock;
    }
}
