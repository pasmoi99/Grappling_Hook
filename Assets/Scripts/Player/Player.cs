using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _left;
    private bool _right;
    private bool _jump;
    private Rigidbody2D _playerRb;

    [SerializeField] private float _speed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            _left = true;
        }
        else
        {
            _left = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _right = true;
        }
        else
        {
            _right = false;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            _jump = true;
        }
        else
        {
            _jump = false;
        }
    }

    private void FixedUpdate()
    {
        if (_left && _right)
        {
            _playerRb.velocity = new Vector2(0, _playerRb.velocity.y);
        }
        else if (_left)
        {
            if (_playerRb.velocity.x - _acceleration >= -_maxSpeed)
            {
                _playerRb.velocity = new Vector2(_playerRb.velocity.x - _acceleration, 0);
            }
        }
        else if (_right)
        {
            if (_playerRb.velocity.x + _acceleration <= _maxSpeed)
            {
                _playerRb.velocity = new Vector2(_playerRb.velocity.x + _acceleration, 0);
            }
        }
        else
        {
            _playerRb.velocity = new Vector2(0, _playerRb.velocity.y);
        }
    }
}
