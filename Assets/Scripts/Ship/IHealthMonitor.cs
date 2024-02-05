using System;

internal interface IHealthMonitor
{
    public Action<int> OnHealthChanged
    {
        get;
        set;
    }
}