using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isMovingLeft;
    private bool _isMovingRight;
    private bool _isJumping;
    private bool _canJump;

    private float _lastPosY;
    private float _currentPosY;
    private float _playerSpriteSizeY;

    private LayerMask _wall;

    private Vector2 _playerSpriteCenter;
    private Vector2 _playerSpriteBottomLeft;
    private Vector2 _playerSpriteBottomCenter;

    private RaycastHit2D _hit;

    private Rigidbody2D _playerRb;

    [SerializeField] private float _jumpForce;
    [SerializeField] private float _radiusGroundDetection;
    //[SerializeField] private float _jumpTimerTrue;
    //[SerializeField] private float _jumpTimerMax;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;

    private void Start()
    {
        _isMovingLeft = false;
        _isMovingRight = false;
        _isJumping = false;
        _canJump = true;

        _playerSpriteSizeY = GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.localScale.y;

        _wall = MainGame.Main.WallMask;

        _playerSpriteCenter = GetComponent<SpriteRenderer>().bounds.center;
        _playerSpriteBottomLeft = GetComponent<SpriteRenderer>().bounds.min;
        _playerSpriteBottomCenter = new Vector2(_playerSpriteCenter.x, _playerSpriteBottomLeft.y);
        
        _playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _playerSpriteCenter = GetComponent<SpriteRenderer>().bounds.center;
        _playerSpriteBottomLeft = GetComponent<SpriteRenderer>().bounds.min;
        _playerSpriteBottomCenter = new Vector2(_playerSpriteCenter.x, _playerSpriteBottomLeft.y);
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

        //_lastPosY = _currentPosY;
        //_currentPosY = _playerRb.position.y;

        //Physics2D.OverlapCircle(transform.position - new Vector3(transform.position.x, _playerSpriteSizeY / 2),_radiusGroundDetection);

        if (Physics2D.OverlapCircle(_playerSpriteBottomCenter, _radiusGroundDetection,_wall) != null)
        {
            _canJump = true;
        }
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W)) && _canJump)
        {
            _isJumping = true;
        }
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(_playerSpriteBottomCenter,_radiusGroundDetection);
    //}
    private void FixedUpdate()
    {
       
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

        if (_isJumping)
        {
            _isJumping = false;
            _playerRb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _canJump = false;
        }
    }

    public Vector3 GetPlayerRBVelocity()
    {
        return _playerRb.velocity;
    } 
}
