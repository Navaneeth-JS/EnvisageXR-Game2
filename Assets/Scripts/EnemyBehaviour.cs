using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EnemyBehaviour : MonoBehaviour
{
    GameManager _gameManager;
    PlayerController _player;

    Rigidbody2D _rb;

    float _enemyHealth = 100f;
    float _enemyMoveSpeed = 2f;

    Vector2 _moveDirection;
    Vector2 _enemyVelocity;
    bool _disableEnemy = false;
    TextMeshProUGUI ScoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        ScoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        ScoreText.text = "Score: " + _gameManager._score;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameManager._gameOver && !_disableEnemy)
        {
            MoveEnemy();
            RotateEnemy();
        }
    }

    void GetDirection()
    {
        _moveDirection = _player.transform.position - transform.position;
        _moveDirection.Normalize();
    }

    void MoveEnemy()
    {
        GetDirection();
        _enemyVelocity = _moveDirection * _enemyMoveSpeed;
        _rb.MovePosition(_rb.position + _enemyVelocity * Time.fixedDeltaTime);
    }

    void RotateEnemy()
    {
        GetDirection();
        float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg - 90f;
        _rb.MoveRotation(angle);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            StartCoroutine(Damaged());
            _enemyHealth -= 25;

            if (_enemyHealth <= 0f)
            {
                Destroy(gameObject);
                _gameManager._score += 1;
                ScoreText.text = "Score: " + _gameManager._score;
            }

            Destroy(collision.gameObject);
        }

        else if (collision.gameObject.tag == "Player")
        {
            _gameManager._gameOver = true;
            collision.gameObject.SetActive(false);
            ScoreText.text = "GAME OVER - Final Score: " + _gameManager._score;
        }
    }

    IEnumerator Damaged()
    {
        _disableEnemy = true;
        _rb.AddForce(_moveDirection*-1*5f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        _disableEnemy = false;
    }
}
