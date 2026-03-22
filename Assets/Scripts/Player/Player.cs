using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _timerAnimation;
    private float _timerSpawnSmoke;

    private int _currentIdle;
    private int _currentRun;

    private SpriteRenderer _playerSprite;
   
    private bool _isMovingLeft;
    private bool _isMovingRight;
    private bool _isFacingRight;
    private bool _isJumping;
    private bool _canJump;
    private bool _canSpawnWalkingSmoke;

    private LayerMask _wall;

    private Vector2 _playerSpriteCenter;
    private Vector2 _playerSpriteBottomLeft;
    private Vector2 _playerSpriteBottomCenter;

    private Rigidbody2D _playerRb;

    [SerializeField] private float _jumpForce;
    [SerializeField] private float _radiusGroundDetection;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _animationSpeed;
    [SerializeField] private float _walkSmokeFrequency;
    [SerializeField] private float _offsetSmokeX;
    [SerializeField] private float _offsetSmokeY;
    [SerializeField] private float _acceleration;

    [SerializeField] private List<Sprite> _idleLeft;
    [SerializeField] private List<Sprite> _idleRigth;
    [SerializeField] private List<Sprite> _runLeft;
    [SerializeField] private List<Sprite> _runRight;

    private void Start()
    {
        _timerAnimation = 0;
        _timerSpawnSmoke = 0;

        _currentIdle = 0;
        _currentRun = 0;

        _playerSprite = GetComponent<SpriteRenderer>();

        _isMovingLeft = false;
        _isMovingRight = false;
        _isFacingRight = true;
        _isJumping = false;
        _canJump = true;
        _canSpawnWalkingSmoke = false;

        //_playerSpriteSizeY = GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.localScale.y;

        _wall = MainGame.Main.WallMask;

        _playerSpriteCenter = _playerSprite.bounds.center;
        _playerSpriteBottomLeft = _playerSprite.bounds.min;
        _playerSpriteBottomCenter = new Vector2(_playerSpriteCenter.x, _playerSpriteBottomLeft.y);
        
        _playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _timerAnimation += Time.deltaTime;
        _timerSpawnSmoke += Time.deltaTime;

        _playerSpriteCenter = _playerSprite.bounds.center;
        _playerSpriteBottomLeft = _playerSprite.bounds.min;
        _playerSpriteBottomCenter = new Vector2(_playerSpriteCenter.x, _playerSpriteBottomLeft.y);
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            _isMovingLeft = true;
            _isFacingRight = false;
        }
        else
        {
            _isMovingLeft = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _isMovingRight = true;
            _isFacingRight = true;
        }
        else
        {
            _isMovingRight = false;
        }

        if (Physics2D.OverlapCircle(_playerSpriteBottomCenter, _radiusGroundDetection,_wall) != null)
        {
            _canJump = true;
            _canSpawnWalkingSmoke = true;
        }
        else
        {
            _canJump = false;
            _canSpawnWalkingSmoke = false;
        }
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W)) && _canJump)
        {
            _canJump = false;
            _canSpawnWalkingSmoke = false;
            _isJumping = true;
        }

        if ((!_isMovingLeft && !_isMovingRight) || (_isMovingLeft && _isMovingRight))
        {
            if (_isFacingRight && _playerSprite.sprite != _idleRigth[_currentIdle])
            {
                _playerSprite.sprite = _idleRigth[_currentIdle];
            }
            else if (!_isFacingRight && _playerSprite.sprite != _idleLeft[_currentIdle])
            {
                _playerSprite.sprite = _idleLeft[_currentIdle];
            }
        }

        if (_isMovingLeft)
        {
            if (_timerSpawnSmoke >= _walkSmokeFrequency && _canSpawnWalkingSmoke)
            {
                Smoke s = Instantiate(MainGame.Main.WalkSmokeLeft, new Vector3(transform.position.x + _offsetSmokeX,
                    transform.position.y - _offsetSmokeY, transform.position.z), Quaternion.identity);
                _timerSpawnSmoke = 0;
            }
            if(_playerSprite.sprite != _runLeft[_currentRun])
            {
                _playerSprite.sprite = _runLeft[_currentRun];
            }
        }
        else if (_isMovingRight)
        {
            if (_timerSpawnSmoke >= _walkSmokeFrequency && _canSpawnWalkingSmoke)
            {
                Smoke s = Instantiate(MainGame.Main.WalkSmokeRight, new Vector3 (transform.position.x - _offsetSmokeX,
                    transform.position.y - _offsetSmokeY, transform.position.z), Quaternion.identity);
                _timerSpawnSmoke = 0;
            }
            if (_playerSprite.sprite != _runRight[_currentRun])
            {
                _playerSprite.sprite = _runRight[_currentRun];
            }
        }

        if (_timerAnimation >= _animationSpeed)
        {
            _currentIdle = (_currentIdle+1) % _idleLeft.Count;
            _currentRun = (_currentRun+1) % _runLeft.Count;
            _timerAnimation = 0;
        }
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(_playerSpriteCenter, _radiusGroundDetection);
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
