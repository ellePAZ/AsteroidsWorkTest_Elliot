using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Bullet : MonoBehaviour
{
    [SerializeField] float _velocity;
    [SerializeField] float _lifeTime;

    Vector3 _minBounds;
    Vector3 _maxBounds;

    private void Start()
    {
        Invoke("TimedOut", _lifeTime);
        MovementUtilities.GetMinMaxBounds(out _minBounds, out _maxBounds);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * _velocity * Time.deltaTime;
        transform.position = MovementUtilities.GetScreenWrapPosition(_minBounds, _maxBounds, transform);
    }

    void TimedOut()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var healthSubtractable = collision.GetComponent<IHealthSubtractable>();

        if (healthSubtractable != null)
        {
            healthSubtractable.RemoveHealth();
        }

        Destroy(gameObject);
    }
}
