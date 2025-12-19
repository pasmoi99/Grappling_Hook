using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Player _player;

    [SerializeField,Range(1,100)]protected float p_percentDistanceFromPlayer;

    protected void Start()
    {
        _player = MainGame.Main.player;
    }

    protected void Update()
    {
        Vector2 mouse = Input.mousePosition;
        transform.position = _player.transform.position + ((Camera.main.ScreenToWorldPoint(mouse)-_player.transform.position) * (p_percentDistanceFromPlayer / 100));
    }
}
