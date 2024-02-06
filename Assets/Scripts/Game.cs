using Spawning;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    private enum GameState
    {
        Loading,
        Start,
        Playing,
        NextRound,
        GameOver
    }

    List<IAddressableLoader> _addressableLoaders;
    IAsteroidSpawner _asteroidSpawner;

    ISpawner _shipSpawner;
    IUpdateable _shipSpawnerUpdate;

    [Header("Component references")]
    [SerializeField] PlayerLivesIndicator _playerLivesIndicator;
    [SerializeField] GameStateUI _gameStateUI;
    [SerializeField] WaveData[] _waveData;

    [Header("Variables")]
    [SerializeField] float _playerSpawnInterval;
    [SerializeField] float _playerSpawnCircleRadius;

    GameState _state;

    byte _waveCount;
    byte _loadedAssets;

    short _playerLives;

    private void Start()
    {
        _state = GameState.Start;
        _gameStateUI.SetStartGameText();

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
        Time.timeScale = 0f;
    }

    private void Update()
    {
        _shipSpawnerUpdate.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (_state)
            {
                case GameState.Start:
                case GameState.NextRound:
                    {
                        _state = GameState.Playing;
                        _gameStateUI.ToggleText(false);
                        PlayGame();
                        NextLevel();
                    }
                    break;
                case GameState.GameOver:
                    {
                        _gameStateUI.SetStartGameText();
                    }
                    break;
                default:
                    break;
            }
        }        
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
            loader.LoadAddressables(AssetLoaded);
    }

    void AssetLoaded()
    {
        _loadedAssets++;
        if (_loadedAssets == _addressableLoaders.Count)
        {
            Debug.Log("Game Loaded");
        }
    }

    void NextLevel()
    {
        for (int i = 0; i < _waveData[_waveCount].asteroidCount; i++)
            _asteroidSpawner.Spawn(Enemies.AsteroidType.Small, null);

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
            _gameStateUI.SetWinText();
            Debug.Log("You win!");
        }
        else
        {
            PauseGame();
            _gameStateUI.SetNextRoundText();
            _state = GameState.NextRound;
        }
    }

    void PauseGame() => Time.timeScale = 0f;
    void PlayGame() => Time.timeScale = 1f;

    void PlayerHealthLoss(object context)
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
