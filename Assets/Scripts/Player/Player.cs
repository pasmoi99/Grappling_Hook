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
    private bool _hasPlayedLandingSound;
    private bool _canPlayWalkingSound;
    private bool _hasSwappedColliderPos;

    private LayerMask _wall;

    private Vector2 _playerSpriteCenter;
    private Vector2 _playerSpriteBottomLeft;
    private Vector2 _playerSpriteBottomRight;
    private Vector2 _playerSpriteBottomCenter;

    private BoxCollider2D _collider2D;

    private Rigidbody2D _playerRb;

    private AudioSource _playerSource;

    [SerializeField] private float _jumpForce;
    [SerializeField] private float _radiusGroundDetection;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _animationSpeed;
    [SerializeField] private float _walkSmokeFrequency;
    [SerializeField] private float _offsetSmokeX;
    [SerializeField] private float _offsetSmokeY;
    [SerializeField] private float _acceleration;

    [SerializeField] private List<Sprite> _idleLeft;
    [SerializeField] private List<Sprite> _idleRight;
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
        _canJump = false;
        _canSpawnWalkingSmoke = false;
        _hasPlayedLandingSound = false;
        _canPlayWalkingSound = false;
        _hasSwappedColliderPos = true;

        //_playerSpriteSizeY = GetComponent<SpriteRenderer>().sprite.bounds.size.y * transform.localScale.y;

        _wall = MainGame.Main.WallMask;

        _playerSpriteCenter = _playerSprite.bounds.center;
        _playerSpriteBottomLeft = _playerSprite.bounds.min;
        _playerSpriteBottomRight = new Vector2 (_playerSprite.bounds.extents.x, _playerSprite.bounds.min.y);
        _playerSpriteBottomCenter = new Vector2(_playerSpriteCenter.x, _playerSpriteBottomLeft.y);
        
        _collider2D = GetComponent<BoxCollider2D>();

        _playerRb = GetComponent<Rigidbody2D>();

        _playerSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        _timerAnimation += Time.deltaTime;
        _timerSpawnSmoke += Time.deltaTime;

        _playerSpriteCenter = _playerSprite.bounds.center;
        _playerSpriteBottomLeft = _playerSprite.bounds.min;
        _playerSpriteBottomRight = new Vector2(_playerSprite.bounds.max.x, _playerSprite.bounds.min.y);
        _playerSpriteBottomCenter = new Vector2(_playerSpriteCenter.x, _playerSpriteBottomLeft.y);

        //Debug.Log(_hasSwappedColliderPos);

        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A)) && _isFacingRight)
        {
            _hasSwappedColliderPos = false;
        }

        else if (Input.GetKeyDown(KeyCode.D) && !_isFacingRight)
        {
            _hasSwappedColliderPos = false;
        }

        if (!_hasSwappedColliderPos)
        {
            _hasSwappedColliderPos = true;
            _collider2D.offset = new Vector2 (_collider2D.offset.x * -1,0);
        }

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
        {
            if (_canPlayWalkingSound)
            {
                PlayStepSound(SoundManager.Sounds.Steps);
            }
            _isMovingLeft = true;
            _isFacingRight = false;
            _isMovingRight = false;
        }

        else if (Input.GetKey(KeyCode.D))
        {
            if (_canPlayWalkingSound)
            {
                PlayStepSound(SoundManager.Sounds.Steps);
            }
            _isMovingLeft = false;
            _isMovingRight = true;
            _isFacingRight = true;
        }

        else
        {
            _isMovingLeft = false;
            _isMovingRight = false;
        }

        if (_isFacingRight && Physics2D.OverlapCircle(new Vector2((_playerSpriteBottomCenter.x+_playerSpriteBottomLeft.x)/2, (_playerSpriteBottomCenter.y + _playerSpriteBottomLeft.y) / 2),
            _radiusGroundDetection, _wall) != null)
        {
            if (!_hasPlayedLandingSound)
            {
                _hasPlayedLandingSound = true;
                _playerSource.PlayOneShot(SoundManager.Sounds.Landing);
            }
            _canJump = true;
            _canSpawnWalkingSmoke = true;
            _canPlayWalkingSound = true;
        }
        else if (!_isFacingRight && Physics2D.OverlapCircle(new Vector2((_playerSpriteBottomCenter.x + _playerSpriteBottomRight.x) / 2, (_playerSpriteBottomCenter.y + _playerSpriteBottomRight.y) / 2),
            _radiusGroundDetection, _wall) != null)
        {
            if (!_hasPlayedLandingSound)
            {
                _hasPlayedLandingSound = true;
                _playerSource.PlayOneShot(SoundManager.Sounds.Landing);
            }
            _canJump = true;
            _canSpawnWalkingSmoke = true;
            _canPlayWalkingSound = true;
        }
        else
        {
            _canPlayWalkingSound = false;
            _hasPlayedLandingSound = false;
            _canJump = false;
            _canSpawnWalkingSmoke = false;
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W)) && _canJump)
        {
            _canJump = false;
            _canSpawnWalkingSmoke = false;
            _isJumping = true;
        }

        if ((!_isMovingLeft && !_isMovingRight) || (_isMovingLeft && _isMovingRight))
        {
            if (_isFacingRight && _playerSprite.sprite != _idleRight[_currentIdle])
            {
                _playerSprite.sprite = _idleRight[_currentIdle];
            }
            else if (!_isFacingRight && _playerSprite.sprite != _idleLeft[_currentIdle])
            {
                _playerSprite.sprite = _idleLeft[_currentIdle];
            }
        }

        else if (_isMovingLeft)
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
            _currentIdle = (_currentIdle+1) % _idleRight.Count;
            _currentRun = (_currentRun+1) % _runRight.Count;
            _timerAnimation = 0;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawSphere(_playerSpriteBottomCenter, _radiusGroundDetection);
        
    }
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

        if (_isJumping )
        {
            _isJumping = false;
            _playerRb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _canJump = false;
        }
        
    }

    private void PlayStepSound(List<AudioClip> Steps)
    {
        if (!_playerSource.isPlaying || _playerSource.clip == null)
        {
            int rand = Random.Range(0, Steps.Count);
            _playerSource.clip = Steps[rand];
            _playerSource.Play();
        }
    }

    public Vector3 GetPlayerRBVelocity()
    {
        return _playerRb.velocity;
    } 
}
