using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaucerGunController : MonoBehaviour
{
    [SerializeField] Bullet _bullet;
    [SerializeField] float _fireInterval;

    float _currentTimer;

    private void Start()
    {
        _currentTimer = 0;
    }

    private void Update()
    {
        _currentTimer += Time.deltaTime;
        if (_currentTimer > _fireInterval)
        {
            Instantiate(_bullet, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            _currentTimer = 0;
        }
    }
}
