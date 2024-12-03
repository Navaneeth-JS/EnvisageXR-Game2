using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    Rigidbody2D _rb;
    Camera _mainCamera;

    float _moveVertical;
    float _moveHorizontal;
    float _moveSpeed = 5f;
    float _speedLimiter = 0.7f;
    Vector2 _moveVelocity;

    Vector2 _mousePos;
    Vector2 _offset;

    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject _bulletSpawn;

    bool _isShooting = false;
    float _bulletSpeed = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        _moveVertical = Input.GetAxisRaw("Vertical");
        _moveHorizontal = Input.GetAxisRaw("Horizontal");

        _moveVelocity = new Vector2(_moveHorizontal, _moveVertical) * _moveSpeed;

        if (Input.GetMouseButtonDown(0))
        {
            _isShooting = true;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();

        if (_isShooting)
        {
            StartCoroutine(Fire());
        }
    }

    void MovePlayer()
    {
        if (_moveHorizontal != 0 || _moveVertical != 0)
        {
            if (_moveHorizontal != 0 && _moveVertical != 0)
            {
                _moveVelocity *= _speedLimiter;
            }
            _rb.linearVelocity = _moveVelocity;
        }
        else
        {
            _moveVelocity = new Vector2(0f, 0f);
            _rb.linearVelocity = _moveVelocity;
        }
    }

    void RotatePlayer()
    {
        _mousePos = Input.mousePosition;
        Vector3 screenPoint = _mainCamera.WorldToScreenPoint(transform.localPosition);
        _offset = new Vector2(_mousePos.x - screenPoint.x, _mousePos.y - screenPoint.y).normalized;

        float angle = Mathf.Atan2(_offset.y, _offset.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    IEnumerator Fire()
    {
        _isShooting = false;
        GameObject bullet = Instantiate(_bullet, _bulletSpawn.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = _offset * _bulletSpeed;
        yield return new WaitForSeconds(3);
        Destroy(bullet);
    }

}
