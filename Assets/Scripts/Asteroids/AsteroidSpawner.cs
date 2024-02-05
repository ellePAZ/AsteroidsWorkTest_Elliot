using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Spawning
{
    public class AsteroidSpawner : ISpawner, IAddressableLoader
    {
        const byte MAX_ASTEROIDS = 128;
        const byte MIN_SPAWN_POINT = 1;

        string _assetsLabel;
        int _noSpawnRadius;

        GameObject _smallAsteroidReference;
        GameObject _mediumAsteroidReference;
        GameObject _bigAsteroidReference;

        byte _asteroidsCount;

        Action onAssetsLoaded;
        Action IAddressableLoader.OnAssetsLoaded { get => onAssetsLoaded; }

        Action onLevelFinished;

        public AsteroidSpawner(Action onLevelFinishedCallback)
        {
            _assetsLabel = "Asteroid";
            onLevelFinished = onLevelFinishedCallback;
            _noSpawnRadius = 2;
            _asteroidsCount = 0;
        }

        public void LoadAddressables(Action callback)
        {
            onAssetsLoaded = callback;
            Addressables.LoadAssetsAsync<GameObject>(_assetsLabel, ProcessAddressable).Completed += OnAddressableAssetLoaded;
        }

        void ProcessAddressable(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "SmallAsteroid":
                    _smallAsteroidReference = gameObject;
                    break;
                case "MediumAsteroid":
                    _mediumAsteroidReference = gameObject;
                    break;
                case "BigAsteroid":
                    _bigAsteroidReference = gameObject;
                    break;
            }
        }

        void OnAddressableAssetLoaded(AsyncOperationHandle<IList<GameObject>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                onAssetsLoaded?.Invoke();
            else
                Debug.LogError(handle.Result.ToString());
        }

        //TODO: Add unit test to make sure that creating and destroying keeps count at proper amount
        //TODO: Maybe add unit test to make sure that asteroids don't spawn in the players area
        public void Spawn()
        {
            if (_bigAsteroidReference != null)
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

                var asteroidObject = GameObject.Instantiate(_bigAsteroidReference, spawnPos, Quaternion.identity);
                var deathObservable = asteroidObject.GetComponent<IDeathObservable>();
                Debug.Assert(deathObservable != null, "There is no death observable on the asteroid prefab, please fix");
                deathObservable.Subscribe(UnSpawn);

                _asteroidsCount++;
            }
        }

        public void UnSpawn()
        {
            Debug.Assert(_asteroidsCount > 0, "You are trying to destroy an asteroid that doesn't exist");

            _asteroidsCount--;

            if (_asteroidsCount == 0)
            {
                onLevelFinished?.Invoke();
            }
        }
    }
}
