using Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShipSpawner : ISpawner, IAddressableLoader, IUpdateable
{
    const string ASSET_STRING = "Assets/Prefabs/Player/PlayerShip.prefab";

    GameObject _shipReference;
    GameObject _ship;

    Action _onAssetsLoaded;
    Action IAddressableLoader.OnAssetsLoaded { get => _onAssetsLoaded; }

    Action<object> _playerKilledCallback;

    bool _trySpawnPlayer;
    float _currentTimer;
    float _spawnInterval;

    int _playerSpawnRadius;

    public ShipSpawner(Action<object> onPlayerDeathCallback, float spawnInterval)
    {
        _playerKilledCallback = onPlayerDeathCallback;
        _spawnInterval = spawnInterval;
        _currentTimer = 0f;
        _playerSpawnRadius = 2;
        _trySpawnPlayer = false;
    }

    public void LoadAddressables(Action callback)
    {
        _onAssetsLoaded = callback;
        Addressables.LoadAssetAsync<GameObject>(ASSET_STRING).Completed += OnAddressableAssetLoaded;
    }

    void OnAddressableAssetLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _shipReference = handle.Result;
            _onAssetsLoaded?.Invoke();
        }
        else
        {
            Debug.LogError(handle.Result.ToString());
        }
    }

    public void CreateShip()
    {
        Debug.Assert(_ship == null, "Cannot spawn a ship when it already exists");
        Debug.Assert(_shipReference != null, "Cannot spawn ship without a valid ship reference");

        var camera = Camera.main;
        Vector2 center = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2));
        _ship = GameObject.Instantiate(_shipReference, center, Quaternion.identity);
        _ship.GetComponent<IDeathObservable>().Subscribe(_playerKilledCallback);
    }

    void CheckForPlayerSpawn()
    {
        if (Physics2D.OverlapCircle(Vector2.zero, _playerSpawnRadius) != null)
        {
            _currentTimer = _spawnInterval;
        }
        else
        {
            CreateShip();
            _trySpawnPlayer = false;
            _currentTimer = 0f;
        }
    }

    public void Spawn()
    {
        _trySpawnPlayer = true;
    }

    public void UnSpawn()
    {
        Debug.Assert(_ship != null, "Unspawn was called when ship was null");
        GameObject.Destroy(_ship);
    }

    public void Update(float deltaTime)
    {
        if (_trySpawnPlayer)
        {
            _currentTimer -= Time.deltaTime;
            if (_currentTimer <= 0)
            {
                CheckForPlayerSpawn();
            }
        }
    }
}
