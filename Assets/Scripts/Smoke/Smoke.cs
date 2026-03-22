using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    [SerializeField] private float _animationSpeed;
    [SerializeField] private List<Sprite> _smokeSprites;

    private int _currentSprite;
    private float _timer;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _currentSprite = 0;
        _timer = 0;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _animationSpeed)
        {
            _currentSprite++;
            _spriteRenderer.sprite = _smokeSprites[_currentSprite];
            _timer = 0;
        }

        if (_currentSprite == _smokeSprites.Count-1)
        {
            Destroy(gameObject);
        }
    }
}
