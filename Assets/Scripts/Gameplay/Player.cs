using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float _speed = 5f;
    private float _speedMultiplier = 2f;
    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField] private float _powerupTime = 5.0f;
    
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject[] _engines;
    private int _firstEngineIndex;

    [SerializeField] private AudioManager _audioManager;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private Animator _anim;
    
    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;

    private float turnDamp = 0.1f;
    private float _damageCooldown = 0.2f;
    private float _lastDamageTime = -999f;
    private int _lives = 3;
    private int _score;

    IEnumerator _coroutine;

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _anim = GetComponent<Animator>();

        if (_spawnManager == null)
        {
            Debug.Log("The Spawn Manager is NULL.");
        }
        if (_uiManager == null)
        {
            Debug.Log("The UI Manager is NULL.");
        }

        _firstEngineIndex = Random.Range(0, 2);
    }

    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();

        float h = Input.GetAxisRaw("Horizontal");

        _anim.SetFloat("Turn", h, turnDamp, Time.deltaTime);
    }


    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 diraction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(diraction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        Vector3 posOfset = transform.position + new Vector3(0, 1.05f, 0);

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, posOfset, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, posOfset, Quaternion.identity);
        }

        _audioManager.PlayLaserSound();
    }

    public void Damage()
    {
        if (Time.time - _lastDamageTime < _damageCooldown)
            return;

        _lastDamageTime = Time.time;

        if (_isShieldActive == true)
        {
            _shieldVisualizer.SetActive(false);
            _isShieldActive = false;
            return;
        }

        _lives--;   

        if (_lives == 2)
        {
            _engines[_firstEngineIndex].SetActive(true);
        }
        else if (_lives == 1)
        {
            _engines[1- _firstEngineIndex].SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _audioManager.ExplosionSound();
            _spawnManager.OnPlayerDeath();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        _audioManager.PowerUpSound();

        _coroutine = TripleShotPowerDownRoutine(_powerupTime);
        StartCoroutine(_coroutine);
    }
    IEnumerator TripleShotPowerDownRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        _audioManager.PowerUpSound();

        _coroutine = SpeedBoostPowerDownRoutine(_powerupTime);
        StartCoroutine(_coroutine);
    }

    IEnumerator SpeedBoostPowerDownRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _speed /= _speedMultiplier;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _audioManager.PowerUpSound();

        _shieldVisualizer.SetActive(true);
        _coroutine = ShieldPowerDownRoutine(10F);
        StartCoroutine(_coroutine);
    }

    IEnumerator ShieldPowerDownRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _shieldVisualizer.SetActive(false);
        _isShieldActive = false;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
