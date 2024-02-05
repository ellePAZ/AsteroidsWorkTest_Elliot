using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public interface IAddressableLoader
{
    Action OnAssetsLoaded 
    {
        get;
    }

    public void LoadAddressables(Action callback);
}
