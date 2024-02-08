using System;
using UnityEngine;

namespace Ship
{
    public class ShipHurtbox : MonoBehaviour, IDeathObservable
    {
        Action<object> onKilled;
        Action<object> IDeathObservable.OnKilled { get => onKilled; set => onKilled = value; }

        public void Subscribe(Action<object> onKilled) => this.onKilled += onKilled;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onKilled?.Invoke(this);
        }
    }
}
