using Enemies;
using Spawning;
using System.Collections.Generic;
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
	IAsteroidSpawner _asteroidSpawner;

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
	[SerializeField] uint _scorePerExtraLife;
	[SerializeField] byte _playerStartingLives;

	GameState _state;

	byte _asteroidsCount;
	byte _waveCount;
	byte _loadedAssets;

	short _playerLives;

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

		_addressableLoaders = new();

		AsteroidSpawner asteroidSpawner = new AsteroidSpawner(LevelFinished, AsteroidDestroyed);
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
			_asteroidSpawner.Spawn(Enemies.AsteroidType.Small, null);

		_shipSpawner.Spawn();
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

	void AsteroidDestroyed(AsteroidType asteroidType)
	{
		_scoreKeeper.AddScore(_scoreSheet.GetScore(asteroidType));

		if (_scoreKeeper.GetScore() % _scorePerExtraLife  == 0)
		{
			_playerLives++;
			_playerLivesIndicator.SetLives(_playerLives);
		}
	}
}
