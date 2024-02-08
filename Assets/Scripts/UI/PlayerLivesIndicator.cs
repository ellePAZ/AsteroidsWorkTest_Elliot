using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLivesIndicator : MonoBehaviour
{
    [SerializeField] GameObject _playerLifePrefab;
    [SerializeField] int _initialLivesCapacity = 10;

    List<GameObject> _playerLivesList;

    int _currentLivesCount;

    private void Awake()
    {
        _playerLivesList = new List<GameObject>(_initialLivesCapacity);

        for (int i = 0; i < _initialLivesCapacity; i++)
        {
            var newLife = Instantiate(_playerLifePrefab, transform);
            _playerLivesList.Add(newLife);

            _playerLivesList[i].gameObject.SetActive(false);
        }
    }

    public void SetLives(int count)
    {
        _currentLivesCount = count;

        for (int i = 0; i < count; i++)
            _playerLivesList[i].SetActive(true);
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
