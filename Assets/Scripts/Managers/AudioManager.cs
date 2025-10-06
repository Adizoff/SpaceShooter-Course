using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] _laserSouces;
    [SerializeField] private AudioSource _explosionSource;
    [SerializeField] private AudioSource _powerupSource;
    private int _index = 0;

    public void PlayLaserSound()
    {
        if (_laserSouces == null || _laserSouces.Length == 0) return;

        _laserSouces[_index].Play();
        _index = (_index + 1) % _laserSouces.Length;
    }

    public void ExplosionSound()
    {
        _explosionSource.Play();
    }

    public void PowerUpSound()
    {
        _powerupSource.Play();
    }
}
