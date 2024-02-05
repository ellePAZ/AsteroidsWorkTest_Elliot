using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLivesIndicator : MonoBehaviour
{
    [SerializeField] GameObject _playerLifePrefab;
    [SerializeField] int _initialLivesCount = 10;

    List<GameObject> _playerLivesList;

    int _currentLivesCount;

    public void SetupLives(int count)
    {
        _currentLivesCount = count;

        _playerLivesList = new List<GameObject>(_initialLivesCount);

        for (int i = 0; i < _initialLivesCount; i++)
        {
            var newLife = Instantiate(_playerLifePrefab, transform);
            _playerLivesList.Add(newLife);
            
            if (i > _currentLivesCount - 1)
            {
                newLife.SetActive(false);
            }
        }
    }

    public void AddLife()
    {
        _currentLivesCount++;

        if (_currentLivesCount < _playerLivesList.Count)
            _playerLivesList[_currentLivesCount].SetActive(true);
        else
            _playerLivesList.Add(Instantiate(_playerLifePrefab, transform));

        _currentLivesCount++;
    }

    public void RemoveLife()
    {
        _currentLivesCount--;
        _playerLivesList[_currentLivesCount].SetActive(false);
    }
}
