using System;
using UnityEngine;

public class ShipHealth : MonoBehaviour, IHealthSubtractable, IHealthAddable, IHealthMonitor
{
    int _health = 3;

    Action<int> onHealthChanged;
    Action<int> IHealthMonitor.OnHealthChanged { get => onHealthChanged; set => onHealthChanged = value; }

    private void OnDestroy()
    {
        this.onHealthChanged -= onHealthChanged;
    }

    public void AddHealth()
    {
        _health++;
        onHealthChanged?.Invoke(_health);
    }

    public void RemoveHealth()
    {
        _health--;
        onHealthChanged?.Invoke(_health);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RemoveHealth();
    }
}
