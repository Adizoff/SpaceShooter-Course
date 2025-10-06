using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 3.0f;
    [SerializeField]
    private GameObject _explosionPrefab;


    private SpawnManager _spawnManager;
    private CircleCollider2D _collider;
    private AudioManager _audioManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        _collider = GetComponent<CircleCollider2D>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
        if (_audioManager == null)
        {
            Debug.LogError("The Audio Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Laser")
        {
            _collider.enabled = false;

            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _audioManager.ExplosionSound();
            Destroy(other.gameObject);
            
            _spawnManager.StartSpawning();

            Destroy(this.gameObject, 0.25f);
        }
    }
}
