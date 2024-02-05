using Spawning;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    List<IAddressableLoader> _addressableLoaders;
    ISpawner _asteroidSpawner;

    ISpawner _shipSpawner;
    IUpdateable _shipSpawnerUpdate;

    [Header("Component references")]
    [SerializeField] PlayerLivesIndicator _playerLivesIndicator;
    [SerializeField] WaveData[] _waveData;

    [Header("Variables")]
    [SerializeField] float _playerSpawnInterval;
    [SerializeField] float _playerSpawnCircleRadius;

    byte _waveCount;
    byte _loadedAssets;

    short _playerLives;

    private void Start()
    {
        _loadedAssets = 0;
        _waveCount = 0;
        _playerLives = 3;

        _addressableLoaders = new();

        AsteroidSpawner asteroidSpawner = new AsteroidSpawner(LevelFinished);
        _addressableLoaders.Add(asteroidSpawner);
        _asteroidSpawner = asteroidSpawner;

        ShipSpawner shipSpawner = new ShipSpawner(PlayerHealthLoss, 1f);
        _addressableLoaders.Add(shipSpawner);
        _shipSpawner = shipSpawner;
        _shipSpawnerUpdate = shipSpawner;

        StartLoadingAssets();

        //TODO: Change this to not be a magic number
        _playerLivesIndicator.SetupLives(3);
    }

    private void Update()
    {
        _shipSpawnerUpdate.Update(Time.deltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(Vector2.zero, _playerSpawnCircleRadius);
    }
#endif

    void StartLoadingAssets()
    {
        foreach (var loader in _addressableLoaders)
        {
            loader.LoadAddressables(AssetLoaded);
        }
    }

    void AssetLoaded()
    {
        _loadedAssets++;
        if (_loadedAssets == _addressableLoaders.Count)
        {
            NextLevel();
            Debug.Log("Game Starting");
        }
    }

    void NextLevel()
    {
        for (int i = 0; i < _waveData[_waveCount].asteroidCount; i++)
            _asteroidSpawner.Spawn();

        _shipSpawner.Spawn();
    }

    void LevelFinished()
    {
        //TODO: Load next level
        _waveCount++;
        _shipSpawner.UnSpawn();

        if (_waveCount == _waveData.Length)
        {
            // Win State
            Debug.Log("You win!");
        }
        else
        {
            NextLevel();
        }
    }

    void PlayerHealthLoss()
    {
        _playerLives--;

        _playerLivesIndicator.RemoveLife();
        _shipSpawner.UnSpawn();

        if (_playerLives > 0)
        {
            _shipSpawner.Spawn();
        }
        else
        {
            // Game Over
        }
    }
}
