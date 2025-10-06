using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 4f;
    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;
    private Animator _anim;
    private AudioManager _audioManager;
    private BoxCollider2D _collider;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    private bool _isEnemyAlive = true;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }
        if (_audioManager == null)
        {
            Debug.LogError("The Audio Manager is NULL.");
        }

        _anim = GetComponent<Animator>();

        _collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _canFire && _isEnemyAlive == true)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (_player != null)
            {
                _player.Damage();
            }

            _isEnemyAlive = false;

            _anim.SetTrigger("OnEnemyDeath");
            _audioManager.ExplosionSound();

            _collider.enabled = false;
            _speed = 0;
            Destroy(this.gameObject, 2.4f);
        }
        else if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            
            if (_player != null)
            {
                _player.AddScore(10);
            }

            _isEnemyAlive = false;

            _anim.SetTrigger("OnEnemyDeath");
            _audioManager.ExplosionSound();

            _collider.enabled = false;
            _speed = 0;

            Destroy(this.gameObject, 2.4f);
        }
    }
}
