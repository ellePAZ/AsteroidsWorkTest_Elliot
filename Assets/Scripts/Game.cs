using Spawning;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Game : MonoBehaviour
{
    List<IAddressableLoader> _addressableLoaders;
    ISpawner _asteroidSpawner;
    ISpawner _shipSpawner;

    [SerializeField] WaveData[] _waveData;
    [SerializeField] float _playerSpawnInterval;

    [SerializeField] float _playerSpawnCircleRadius;

    byte _waveCount;
    byte _loadedAssets;

    private void Start()
    {
        _loadedAssets = 0;
        _waveCount = 0;

        _addressableLoaders = new();

        AsteroidSpawner asteroidSpawner = new AsteroidSpawner(LevelFinished);
        _addressableLoaders.Add(asteroidSpawner);
        _asteroidSpawner = asteroidSpawner;

        ShipSpawner shipSpawner = new ShipSpawner(PlayerHealthLoss);
        _addressableLoaders.Add(shipSpawner);
        _shipSpawner = shipSpawner;

        StartLoadingAssets();
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
        _shipSpawner.UnSpawn();

        if (_waveCount == _waveData.Length)
        {
            // Win State
        }
        else
        {
            NextLevel();
        }
    }

    void PlayerHealthLoss(int newHealth)
    {
        //TODO: Tell the UI that the player lost health
        //      Tell the ShipSpawner to unspawn the player
        //      Handle game over if player has <= 0 health
        _shipSpawner.UnSpawn();

        if (newHealth > 0)
        {
            StartCoroutine(CheckForShipSpawn(_playerSpawnInterval));
        }
        else
        {
            // Game Over
        }
    }

    IEnumerator CheckForShipSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Physics2D.OverlapCircle(Vector2.zero, _playerSpawnCircleRadius) != null)
        {
            StartCoroutine(CheckForShipSpawn(delay));
        }
        else
        {
            _shipSpawner.Spawn();
        }
    }
}
