using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Spawning
{
    public class AsteroidSpawner : ISpawner, IAddressableLoader
    {
        const byte MAX_ASTEROIDS = 128;
        const byte MIN_SPAWN_POINT = 1;
        const string ASSET_STRING = "Assets/Prefabs/Asteroids/MediumAsteroid.prefab";

        int _noSpawnRadius;

        GameObject _asteroidReference;

        GameObject[] _asteroidObjects = new GameObject[MAX_ASTEROIDS];
        byte _asteroidsCount;

        Action onAssetsLoaded;
        Action IAddressableLoader.OnAssetsLoaded { get => onAssetsLoaded; }

        float _delay = 0;

        public AsteroidSpawner(Action onLevelFinishedCallback)
        {
            _noSpawnRadius = 2;
            _asteroidsCount = 0;
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
                _asteroidReference = handle.Result;
                onAssetsLoaded?.Invoke();
            }
            else
            {
                Debug.LogError(handle.Result.ToString());
            }
        }

        //TODO: Add unit test to make sure that creating and destroying keeps count at proper amount
        //TODO: Maybe add unit test to make sure that asteroids don't spawn in the players area
        public void Spawn()
        {
            if (_asteroidReference != null)
            {
                Debug.Assert(_asteroidsCount < MAX_ASTEROIDS, "Max asteroid count reached, something went wrong");

                Camera camera = Camera.main;

                Vector2 spawnPos = new Vector2(UnityEngine.Random.Range(MIN_SPAWN_POINT, camera.pixelWidth - 1), UnityEngine.Random.Range(MIN_SPAWN_POINT, camera.pixelHeight));
                spawnPos = camera.ScreenToWorldPoint(spawnPos);

                Vector2 center = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2));
                if (Vector2.Distance(center, spawnPos) < _noSpawnRadius)
                {
                    Vector2 unitVector = spawnPos.normalized;
                    spawnPos = unitVector * _noSpawnRadius;
                    Debug.Log($"New distance from center: {Vector2.Distance(center, spawnPos)}");
                }

                _asteroidObjects[_asteroidsCount] = GameObject.Instantiate(_asteroidReference, spawnPos, Quaternion.identity);
                _asteroidsCount++;
            }
        }

        public void UnSpawn()
        {
            Debug.Assert(_asteroidsCount > 0, "You are trying to destroy an asteroid that doesn't exist");

            GameObject.Destroy(_asteroidObjects[_asteroidsCount - 1]);
            _asteroidsCount--;
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
