using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class AsteroidHealth : MonoBehaviour, IHealthSubtractable
    {
        int _health = 1;

        void IHealthSubtractable.RemoveHealth()
        {
            _health -= 1;

            if (_health <= 0)
                Destroy(gameObject);
        }
    }
}
