using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Spawning
{
    public class AsteroidSpawner : MonoBehaviour
    {
        const byte MAX_ASTEROIDS = 128;

        [SerializeField] AssetReferenceGameObject _asteroid;
        GameObject _asteroidReference;

        GameObject[] _asteroidObjects = new GameObject[MAX_ASTEROIDS];
        byte _asteroidsCount = 0;

        private void Start()
        {
            _asteroid.LoadAssetAsync().Completed += OnAddressableAssetLoaded;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                CreateAsteroid();

            if (Input.GetKeyDown(KeyCode.D))
                DestroyAsteroid();
        }

        void OnAddressableAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _asteroidReference = handle.Result;
                //_asteroid.ReleaseInstance(_asteroidReference);
                CreateAsteroid();
            }
            else
            {
                Debug.LogError(handle.Result.ToString());
            }
        }

        void CreateAsteroid()
        {
            if (_asteroidReference != null)
            {
                Debug.Assert(_asteroidsCount < MAX_ASTEROIDS, "Max asteroid count reached, something went wrong");

                _asteroidObjects[_asteroidsCount] = Instantiate(_asteroidReference);
                _asteroidsCount++;
            }
        }

        void DestroyAsteroid()
        {
            Debug.Assert(_asteroidsCount > 0, "You are trying to destroy an asteroid that doesn't exist");

            Destroy(_asteroidObjects[_asteroidsCount - 1]);
            _asteroidsCount--;
        }
    }
}
