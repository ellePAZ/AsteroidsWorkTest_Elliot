using Enemies;
using System;
using System.Collections.Generic;
using UnityEditor;
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
        public int noSpawnRadius => _noSpawnRadius;

        GameObject _smallAsteroidReference;
        GameObject _mediumAsteroidReference;
        GameObject _bigAsteroidReference;

        byte _asteroidsCount;

        Action onAssetsLoaded;
        Action IAddressableLoader.OnAssetsLoaded { get => onAssetsLoaded; }

        Action onLevelFinished;
        Action<AsteroidType> onAsteroidKilledCallback;

        HashSet<GameObject> _spawnedAsteroids;

        public AsteroidSpawner(Action onLevelFinishedCallback, Action<AsteroidType> onAsteroidKilledCallback)
        {
            onLevelFinished = onLevelFinishedCallback;
            this.onAsteroidKilledCallback = onAsteroidKilledCallback;

            _assetsLabel = "Asteroid";
            _noSpawnRadius = 3;
            _asteroidsCount = 0;

            _spawnedAsteroids = new();
        }

        public void Reset()
        {
            foreach (var asteroid in _spawnedAsteroids)
            {
                GameObject.Destroy(asteroid);
            }

            _spawnedAsteroids.Clear();
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
                var spawnPos = GetNewSpawnPosition(position);

                var asteroidObject = GameObject.Instantiate(asteroidReference, spawnPos, Quaternion.identity);
                var deathObservable = asteroidObject.GetComponent<IDeathObservable>();
                Debug.Assert(deathObservable != null, "There is no death observable on the asteroid prefab, please fix");

                deathObservable.Subscribe(UnSpawn);
                _spawnedAsteroids.Add(asteroidObject);

                _asteroidsCount++;
            }
        }

        public Vector3 GetNewSpawnPosition(Vector3? position)
        {
            Camera camera = Camera.main;

            Vector3 spawnPos = Vector3.zero;
            if (position.HasValue)
            {
                spawnPos = position.Value;
            }
            else
            {
                spawnPos = new Vector2(UnityEngine.Random.Range(MIN_SPAWN_POINT, camera.pixelWidth - 1), UnityEngine.Random.Range(MIN_SPAWN_POINT, camera.pixelHeight));
                spawnPos = camera.ScreenToWorldPoint(spawnPos);
            }

            // Add slight nudge to avoid position of (0,0,0)
            var nudge = new Vector3(0.001f, 0.001f, 0f);
            if (UnityEngine.Random.Range(0, 2) == 0)
                nudge.x *= -1;
            if (UnityEngine.Random.Range(0, 2) == 0)
                nudge.y *= -1;

            spawnPos += nudge;
            spawnPos.z = 0;
            Vector2 center = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2));
            if (Vector2.Distance(center, spawnPos) < _noSpawnRadius)
            {
                Vector2 unitVector = spawnPos.normalized;
                spawnPos = unitVector * _noSpawnRadius;
            }

            return spawnPos;
        }

        public void UnSpawn(object context)
        {
            Debug.Assert(_asteroidsCount > 0, "You are trying to destroy an asteroid that doesn't exist");

            _asteroidsCount--;

            if (context == null)
                return;

            AsteroidHealth asteroidHealth = (AsteroidHealth)context;
            if (asteroidHealth == null)
                return;

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

            var asteroidGameObject = asteroidHealth.gameObject;
            Debug.Assert(_spawnedAsteroids.Contains(asteroidGameObject), "_spawnedAsteroids doesn't contain this asteroid, something is wrong");

            _spawnedAsteroids.Remove(asteroidGameObject);
            GameObject.Destroy(asteroidGameObject);

            onAsteroidKilledCallback?.Invoke(asteroidType);

            if (_asteroidsCount == 0)
                onLevelFinished?.Invoke();
        }
    }
}
