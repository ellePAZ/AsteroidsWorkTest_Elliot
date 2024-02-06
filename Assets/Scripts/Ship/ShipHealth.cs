using System;
using UnityEngine;

namespace Ship
{
    public class ShipHealth : MonoBehaviour, IHealthAddable, IDeathObservable
    {
        int _shipHealth = 3;

        Action<object> onKilled;
        Action<object> IDeathObservable.OnKilled { get => onKilled; set => onKilled = value; }

        public void AddHealth()
        {
            _shipHealth++;
        }

        void RemoveHealth()
        {
            _shipHealth--;
        }

        public void Subscribe(Action<object> onKilled) => this.onKilled += onKilled;

        private void OnTriggerEnter2D(Collider2D collision)
        {

            onKilled?.Invoke(this);
        }
    }
}
