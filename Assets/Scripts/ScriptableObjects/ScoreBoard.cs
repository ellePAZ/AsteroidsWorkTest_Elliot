using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour, IScoreKeeper
{
    [SerializeField] TextMeshProUGUI _scoreText;

    int _currentScore;

    private void Start()
    {
        SetScore(0);
    }

    public void AddScore(int score)
    {
        Debug.Assert(score > 0, "AddScore only takes positive values, if you want to remove score, use RemoveScore()");
        _currentScore += score;
        _scoreText.text = _currentScore.ToString();
    }

    public void RemoveScore(int score)
    {
        Debug.Assert(score < 0, "AddScore only takes positive values, if you want to remove score, use RemoveScore()");
        _currentScore -= score;
        _scoreText.text = _currentScore.ToString();
    }

    public int GetScore() => _currentScore;

    public void SetScore(int score)
    {
        _currentScore = score;
        _scoreText.text = _currentScore.ToString();
    }

}
