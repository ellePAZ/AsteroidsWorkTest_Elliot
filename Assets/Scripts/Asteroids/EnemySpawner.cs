using Enemies;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace Spawning
{
    public class EnemySpawner : IEnemySpawner, IAddressableLoader
    {
        const byte MIN_SPAWN_POINT = 1;
        const byte BIG_ASTEROID_SPLIT_COUNT = 2;
        const byte MEDIUM_ASTEROID_SPLIT_COUNT = 2;

        string _assetsLabel;
        int _noSpawnRadius;
        public int noSpawnRadius => _noSpawnRadius;

        GameObject _smallAsteroidReference;
        GameObject _mediumAsteroidReference;
        GameObject _bigAsteroidReference;
        GameObject _saucerReference;

        HashSet<GameObject> _spawnedAsteroids;

        // bool _saucerSpawned;
        byte _enemiesCount;

        Action onAssetsLoaded;
        Action IAddressableLoader.OnAssetsLoaded { get => onAssetsLoaded; }

        Action onLevelFinished;
        Action<EnemyType> onAsteroidKilledCallback;

        public EnemySpawner(Action onLevelFinishedCallback, Action<EnemyType> onAsteroidKilledCallback)
        {
            onLevelFinished = onLevelFinishedCallback;
            this.onAsteroidKilledCallback = onAsteroidKilledCallback;

            _assetsLabel = "Enemy";
            _noSpawnRadius = 3;
            _enemiesCount = 0;
            //_saucerSpawned = false;

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
                case EnemyType.SmallAsteroid:
                    _smallAsteroidReference = gameObject;
                    break;
                case EnemyType.MediumAsteroid:
                    _mediumAsteroidReference = gameObject;
                    break;
                case EnemyType.LargeAsteroid:
                    _bigAsteroidReference = gameObject;
                    break;
                case EnemyType.Saucer:
                    _saucerReference = gameObject;
                    break;
                default:
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

        public void Spawn(EnemyType enemyType, Vector3? position, bool avoidMiddle)
        {
            GameObject enemyReference = null;
            switch (enemyType)
            {
                case EnemyType.SmallAsteroid:
                    enemyReference = _smallAsteroidReference;
                    break;
                case EnemyType.MediumAsteroid:
                    enemyReference = _mediumAsteroidReference;
                    break;
                case EnemyType.LargeAsteroid:
                    enemyReference = _bigAsteroidReference;
                    break;
                case EnemyType.Saucer:
                    enemyReference = _saucerReference;
                    //if (_saucerSpawned)
                    //    return;
                    break;
            }

            if (enemyReference != null)
            {
                var spawnPos = GetNewSpawnPosition(enemyType, position, avoidMiddle);

                var asteroidObject = GameObject.Instantiate(enemyReference, spawnPos, Quaternion.identity);
                var deathObservable = asteroidObject.GetComponent<IDeathObservable>();
                Debug.Assert(deathObservable != null, "There is no death observable on the asteroid prefab, please fix");

                deathObservable.Subscribe(UnSpawn);
                _spawnedAsteroids.Add(asteroidObject);

                //if (enemyType == EnemyType.Saucer)
                //    _saucerSpawned = true;

                _enemiesCount++;
            }
        }

        public Vector3 GetNewSpawnPosition(EnemyType enemyType, Vector3? position, bool avoidMiddle)
        {
            Vector3 spawnPos = Vector3.zero;
            Camera camera = Camera.main;

            if (enemyType != EnemyType.Saucer)
                GetPositionInsideScreen(ref spawnPos, position, camera, avoidMiddle);
            else
                GetPositionOutsideScreen(ref spawnPos, camera);

            return spawnPos;
        }

        void GetPositionInsideScreen(ref Vector3 spawnPos, Vector3? passedInPosition, Camera camera, bool avoidMiddle)
        {
            if (passedInPosition.HasValue)
            {
                spawnPos = passedInPosition.Value;
            }
            else
            {
                spawnPos = new Vector2(UnityEngine.Random.Range(MIN_SPAWN_POINT, camera.pixelWidth - 1), UnityEngine.Random.Range(MIN_SPAWN_POINT, camera.pixelHeight));
                spawnPos = camera.ScreenToWorldPoint(spawnPos);
            }

            // Add slight nudge to avoid position of (0,0,0)
            var nudge = new Vector3(0.001f, 0.001f, 0f);
            if (UnityEngine.Random.Range(0, 2) == 0)
                nudge.x *= -1f;
            if (UnityEngine.Random.Range(0, 2) == 0)
                nudge.y *= -1f;

            spawnPos += nudge;

            spawnPos.z = 0;

            Vector2 center = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2));

            if (avoidMiddle && Vector2.Distance(center, spawnPos) < _noSpawnRadius)
            {
                Vector2 unitVector = spawnPos.normalized;
                spawnPos = unitVector * _noSpawnRadius;
            }
        }

        public void GetPositionOutsideScreen(ref Vector3 spawnPos, Camera camera)
        {
            var randomPos = UnityEngine.Random.insideUnitCircle.normalized;
            var worldMaxBounds = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth, camera.pixelHeight));
            worldMaxBounds.z = 0;
            var scalar = worldMaxBounds.magnitude;
            randomPos *= scalar;

            spawnPos = randomPos;

            spawnPos.z = 0;
        }

        public void UnSpawn(object context)
        {
            Debug.Assert(_enemiesCount > 0, "You are trying to destroy an asteroid that doesn't exist");

            _enemiesCount--;

            if (context == null)
                return;

            AsteroidHealth asteroidHealth = (AsteroidHealth)context;
            if (asteroidHealth == null)
                return;

            EnemyType asteroidType = asteroidHealth.AsteroidType;

            // Process spawning details
            if (asteroidType != EnemyType.SmallAsteroid)
            {
                var asteroidPosition = asteroidHealth.gameObject.transform.position;
                if (asteroidType == EnemyType.LargeAsteroid)
                    for (byte i = 0; i < BIG_ASTEROID_SPLIT_COUNT; i++)
                        Spawn(EnemyType.MediumAsteroid, asteroidPosition, false);

                if (asteroidType == EnemyType.MediumAsteroid)
                    for (byte i = 0; i < MEDIUM_ASTEROID_SPLIT_COUNT; i++)
                        Spawn(EnemyType.SmallAsteroid, asteroidPosition, false);

                //if (asteroidType == EnemyType.Saucer)
                //    _saucerSpawned = false;
            }

            // Destroy enemy
            var asteroidGameObject = asteroidHealth.gameObject;
            Debug.Assert(_spawnedAsteroids.Contains(asteroidGameObject), "_spawnedAsteroids doesn't contain this asteroid, something is wrong");

            _spawnedAsteroids.Remove(asteroidGameObject);
            GameObject.Destroy(asteroidGameObject);

            onAsteroidKilledCallback?.Invoke(asteroidType);

            if (_enemiesCount == 0)
                onLevelFinished?.Invoke();
        }
    }
}
