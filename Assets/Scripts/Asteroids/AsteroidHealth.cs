using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class AsteroidHealth : MonoBehaviour, IHealthSubtractable, IDeathObservable
    {
        int _health = 1;

        Action onKilled;
        Action IDeathObservable.OnKilled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Subscribe(Action callback) => onKilled += callback;

        void IHealthSubtractable.RemoveHealth()
        {
            _health -= 1;

            if (_health <= 0)
            {
                onKilled?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
