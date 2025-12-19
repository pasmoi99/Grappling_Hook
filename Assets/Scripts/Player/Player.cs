using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isMovingLeft;
    private bool _isMovingRight;
    private bool _canJump=true;
    private float _lastPosY;
    private float _currentPosY;
    private Rigidbody2D _playerRb;

    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpTimerTrue;
    [SerializeField] private float _jumpTimerMax;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    

    

    private void Start()
    {
        _currentPosY = transform.position.y;
        _playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            _isMovingLeft = true;
        }
        else
        {
            _isMovingLeft = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _isMovingRight = true;
        }
        else
        {
            _isMovingRight = false;
        }
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W)) && _canJump)
        {
            _playerRb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _canJump = false;
        }
        _lastPosY = _currentPosY;
        _currentPosY = _playerRb.position.y;
    }

    private void FixedUpdate()
    {
        if (!_canJump && _currentPosY == _lastPosY)
        {
            _canJump = true;
        }
        if (_isMovingLeft && _isMovingRight)
        {
            _playerRb.velocity = new Vector2(0, _playerRb.velocity.y);
        }
        else if (_isMovingLeft)
        {
            if (_playerRb.velocity.x - _acceleration >= -_maxSpeed)
            {
                _playerRb.velocity = new Vector2(_playerRb.velocity.x - _acceleration, _playerRb.velocity.y);
            }
        }
        else if (_isMovingRight)
        {
            if (_playerRb.velocity.x + _acceleration <= _maxSpeed)
            {
                _playerRb.velocity = new Vector2(_playerRb.velocity.x + _acceleration, _playerRb.velocity.y);
            }
        }
        else
        {
            _playerRb.velocity = new Vector2(0, _playerRb.velocity.y);
        }
    }
}
