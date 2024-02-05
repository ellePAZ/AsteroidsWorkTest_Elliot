using Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShipSpawner : ISpawner, IAddressableLoader
{
    const string ASSET_STRING = "Assets/Prefabs/Player/PlayerShip.prefab";

    GameObject _shipReference;
    GameObject _ship;

    Action onAssetsLoaded;
    Action IAddressableLoader.OnAssetsLoaded { get => onAssetsLoaded; }

    Action<int> onPlayerHealthchanged;

    CircleCollider2D _circleCollider;

    public ShipSpawner(Action<int> onPlayerHealthchanged)
    {
        this.onPlayerHealthchanged = onPlayerHealthchanged;
    }

    public void LoadAddressables(Action callback)
    {
        onAssetsLoaded = callback;
        Addressables.LoadAssetAsync<GameObject>(ASSET_STRING).Completed += OnAddressableAssetLoaded;
    }

    void OnAddressableAssetLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _shipReference = handle.Result;
            onAssetsLoaded?.Invoke();
        }
        else
        {
            Debug.LogError(handle.Result.ToString());
        }
    }

    public void Spawn()
    {
        Debug.Assert(_ship == null, "Cannot spawn a ship when it already exists");
        Debug.Assert(_shipReference != null, "Cannot spawn ship without a valid ship reference");

        var camera = Camera.main;
        Vector2 center = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2));
        _ship = GameObject.Instantiate(_shipReference, center, Quaternion.identity);
        _ship.GetComponent<IHealthMonitor>().OnHealthChanged += onPlayerHealthchanged;
    }

    public void UnSpawn()
    {
        Debug.Assert(_ship != null, "Unspawn was called when ship was null");
        GameObject.Destroy(_ship);
    }
}
