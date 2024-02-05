using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float _velocity;

    private void Start()
    {
        Invoke("TimedOut", 1.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.up * _velocity * Time.deltaTime;
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
