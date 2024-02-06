using Enemies;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Spawning
{
    public class AsteroidSpawner : IAsteroidSpawner, IAddressableLoader
    {
        const byte MIN_SPAWN_POINT = 1;
        const byte BIG_ASTEROID_SPLIT_COUNT = 2;
        const byte MEDIUM_ASTEROID_SPLIT_COUNT = 3;

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
            var asteroidHealth = gameObject.GetComponent<AsteroidHealth>();
            Debug.Assert(asteroidHealth != null, "Asteroid health is null, please fix");

            switch (asteroidHealth.AsteroidType)
            {
                case AsteroidType.Small:
                    _smallAsteroidReference = gameObject;
                    break;
                case AsteroidType.Medium:
                    _mediumAsteroidReference = gameObject;
                    break;
                case AsteroidType.Large:
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
        public void Spawn(AsteroidType asteroidType, Vector3? position)
        {
            GameObject asteroidReference = null;
            switch (asteroidType)
            {
                case AsteroidType.Small:
                    asteroidReference = _smallAsteroidReference;
                    break;
                case AsteroidType.Medium:
                    asteroidReference = _mediumAsteroidReference;
                    break;
                case AsteroidType.Large:
                    asteroidReference = _bigAsteroidReference;
                    break;
            }

            if (asteroidType == AsteroidType.Medium)
                asteroidReference = _mediumAsteroidReference;
            else if (asteroidType == AsteroidType.Small)
                asteroidReference = _smallAsteroidReference;

            if (asteroidReference != null)
            {
                Camera camera = Camera.main;

                Vector3 spawnPos;
                if (position != null)
                {
                    spawnPos = position.Value;
                }
                else
                {
                    spawnPos = new Vector2(UnityEngine.Random.Range(MIN_SPAWN_POINT, camera.pixelWidth - 1), UnityEngine.Random.Range(MIN_SPAWN_POINT, camera.pixelHeight));
                    spawnPos = camera.ScreenToWorldPoint(spawnPos);
                }

                spawnPos.z = 0;
                Vector2 center = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2));
                if (Vector2.Distance(center, spawnPos) < _noSpawnRadius)
                {
                    Vector2 unitVector = spawnPos.normalized;
                    spawnPos = unitVector * _noSpawnRadius;
                    Debug.Log($"New distance from center: {Vector2.Distance(center, spawnPos)}");
                }

                var asteroidObject = GameObject.Instantiate(asteroidReference, spawnPos, Quaternion.identity);
                var deathObservable = asteroidObject.GetComponent<IDeathObservable>();
                Debug.Assert(deathObservable != null, "There is no death observable on the asteroid prefab, please fix");
                deathObservable.Subscribe(UnSpawn);

                _asteroidsCount++;
            }
        }

        public void UnSpawn(object context)
        {
            Debug.Assert(_asteroidsCount > 0, "You are trying to destroy an asteroid that doesn't exist");

            _asteroidsCount--;

            if (context != null)
            {
                AsteroidHealth asteroidHealth = (AsteroidHealth)context;
                AsteroidType asteroidType = asteroidHealth.AsteroidType;

                if (asteroidType != AsteroidType.Small)
                {
                    var asteroidPosition = asteroidHealth.gameObject.transform.position;
                    if (asteroidType == AsteroidType.Large)
                        for (byte i = 0; i < BIG_ASTEROID_SPLIT_COUNT; i++)
                            Spawn(AsteroidType.Medium, asteroidPosition);

                    if (asteroidType == AsteroidType.Medium)
                        for (byte i = 0; i < MEDIUM_ASTEROID_SPLIT_COUNT; i++)
                            Spawn(AsteroidType.Small, asteroidPosition);
                }
            }

            if (_asteroidsCount == 0)
                onLevelFinished?.Invoke();
        }
    }
}
