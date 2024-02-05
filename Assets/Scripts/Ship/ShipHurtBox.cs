using System;
using UnityEngine;

namespace Ship
{
    public class ShipHurtBox : MonoBehaviour, IDeathObservable
    {
        Action onKilled;
        Action IDeathObservable.OnKilled { get => onKilled; set => onKilled = value; }

        public void Subscribe(Action onKilled) => this.onKilled += onKilled;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onKilled?.Invoke();
        }
    }
}
