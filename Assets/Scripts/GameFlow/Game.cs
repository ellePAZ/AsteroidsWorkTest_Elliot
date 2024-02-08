using Enemies;
using Spawning;
using System.Collections.Generic;
using System.Threading;
using UI;
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
	IEnemySpawner _asteroidSpawner;

	ISpawner _shipSpawner;
	IUpdateable _shipSpawnerUpdate;
	IScoreKeeper _scoreKeeper;

	[Header("Component references")]
	[SerializeField] PlayerLivesIndicator _playerLivesIndicator;
	[SerializeField] GameStateUI _gameStateUI;
	[SerializeField] ScoreBoard _scoreBoard;

	[SerializeField] WaveData _waveData;
	[SerializeField] ScoreSheet _scoreSheet;

	[SerializeField] InputAction _nextRoundAction;
	[SerializeField] InputAction _retryAction;

	[Header("Variables")]
	[SerializeField] float _playerSpawnInterval;
	[SerializeField] float _playerSpawnCircleRadius;
	[SerializeField] int _scorePerExtraLife;
	[SerializeField] byte _playerStartingLives;

	GameState _state;

	byte _asteroidsCount;
	byte _waveCount;
	byte _loadedAssets;
	short _playerLives;
	float _saucerSpawnTimer;
	int _scoreToNextLife;

    private void OnEnable()
    {
        _nextRoundAction.Enable();
        _retryAction.Enable();
    }

    private void OnDisable()
    {
        _nextRoundAction.Disable();
        _retryAction.Disable();
    }

    private void Start()
	{
		_state = GameState.Loading;
		_gameStateUI.SetLoadingState();

		_loadedAssets = 0;
		_asteroidsCount = _waveData.initialAsteroidCount;
		_waveCount = 0;
		_playerLives = 3;
		_saucerSpawnTimer = 0f;
		_scoreToNextLife = _scorePerExtraLife;

		_addressableLoaders = new();

		EnemySpawner asteroidSpawner = new EnemySpawner(LevelFinished, AsteroidDestroyed);
		_addressableLoaders.Add(asteroidSpawner);
		_asteroidSpawner = asteroidSpawner;

		ShipSpawner shipSpawner = new ShipSpawner(PlayerHealthLoss, 1f);
		_addressableLoaders.Add(shipSpawner);
		_shipSpawner = shipSpawner;
		_shipSpawnerUpdate = shipSpawner;

		_scoreKeeper = _scoreBoard;

		StartLoadingAssets();

		//TODO: Change this to not be a magic number
		_playerLivesIndicator.SetLives(_playerStartingLives);
	}

	private void Update()
	{
		_shipSpawnerUpdate.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.S))
            _asteroidSpawner.Spawn(EnemyType.Saucer, null, false);

        if (_state == GameState.Start || _state == GameState.NextRound) 
		{
			if (_nextRoundAction.WasPressedThisFrame())
			{
				_state = GameState.Playing;
				_gameStateUI.ToggleText(false);
				NextLevel();
			}
		}

		if (_state == GameState.GameOver)
		{
			if (_retryAction.WasPressedThisFrame())
			{
				_gameStateUI.SetStartGameText();
				_asteroidSpawner.Reset();
				_scoreKeeper.SetScore(0);

				_playerLives = _playerStartingLives;
				_playerLivesIndicator.SetLives(_playerLives);

				_waveCount = 0;

				_state = GameState.Start;
			}
		}

		if (_state == GameState.Playing)
		{
			_saucerSpawnTimer += Time.deltaTime;
			if (_saucerSpawnTimer > _waveData.initialShipSpawnInterval)
			{
				_asteroidSpawner.Spawn(EnemyType.Saucer, null, false);
				_saucerSpawnTimer = 0;
			}
		}
	}

	void StartLoadingAssets()
	{
		_gameStateUI.SetLoadingAmount(0);

		foreach (var loader in _addressableLoaders)
			loader.LoadAddressables(AssetLoaded);
	}

	void AssetLoaded()
	{
		_loadedAssets++;
		_gameStateUI.SetLoadingAmount((float)_loadedAssets / (float)_addressableLoaders.Count);
		if (_loadedAssets == _addressableLoaders.Count)
		{
			_state = GameState.Start;
			_gameStateUI.SetStartGameText();
		}
	}

	void NextLevel()
	{
		if (_waveCount != 0)
		{
			if (_waveCount % _waveData.asteroidIncreaseInterval == 0)
				_asteroidsCount++;
		}

		for (int i = 0; i < _asteroidsCount; i++)
			_asteroidSpawner.Spawn(EnemyType.LargeAsteroid, null, true);

		_shipSpawner.Spawn();

		_saucerSpawnTimer = 0f;
	}

	void LevelFinished()
	{
		_waveCount++;
		_shipSpawner.UnSpawn();

		_gameStateUI.SetNextRoundText();
		_state = GameState.NextRound;
	}

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
			// Lose State
			_gameStateUI.SetLoseText();
			_state = GameState.GameOver;
		}
	}

	void AsteroidDestroyed(EnemyType asteroidType)
	{
		var score = _scoreSheet.GetScore(asteroidType);
        _scoreKeeper.AddScore(score);

		_scoreToNextLife -= score;

		if (_scoreToNextLife <= 0)
		{
            _playerLives++;
            _playerLivesIndicator.SetLives(_playerLives);

			_scoreToNextLife += _scorePerExtraLife;
        }
	}
}
