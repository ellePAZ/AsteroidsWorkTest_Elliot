using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public enum EnemyType
    {
        None,
        SmallAsteroid,
        MediumAsteroid,
        LargeAsteroid,
        Saucer,
        Count,
    }

    public class AsteroidHealth : MonoBehaviour, IHealthSubtractable, IDeathObservable
    {
        [SerializeField] EnemyType _asteroidType;
        public EnemyType AsteroidType => _asteroidType;

        Action<object> onKilled;
        Action<object> IDeathObservable.OnKilled { get => onKilled; set => onKilled = value; }

        public void Subscribe(Action<object> callback) => onKilled += callback;

        void IHealthSubtractable.RemoveHealth()
        {
            onKilled?.Invoke(this);
        }
    }
}
