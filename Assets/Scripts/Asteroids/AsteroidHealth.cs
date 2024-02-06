using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public enum AsteroidType
    {
        Small,
        Medium,
        Large,
    }

    public class AsteroidHealth : MonoBehaviour, IHealthSubtractable, IDeathObservable
    {
        [SerializeField] AsteroidType _asteroidType;
        public AsteroidType AsteroidType => _asteroidType;

        int _health = 1;

        Action<object> onKilled;
        Action<object> IDeathObservable.OnKilled { get => onKilled; set => onKilled = value; }

        public void Subscribe(Action<object> callback) => onKilled += callback;

        void IHealthSubtractable.RemoveHealth()
        {
            _health -= 1;

            if (_health <= 0)
            {
                onKilled?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}
