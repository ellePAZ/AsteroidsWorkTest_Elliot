using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreSheet", menuName = "Asteroids/ScoreSheet")]
public class ScoreSheet : ScriptableObject
{
    [System.Serializable]
    public class AsteroidScorePair
    {
        public AsteroidType type;
        public int score;
    }

    public List<AsteroidScorePair> _asteroidTable;

    public int GetScore(AsteroidType type)
    {
        int score = 0;
        for (int i = 0; i < _asteroidTable.Count; i++)
        {
            if (_asteroidTable[i].type == type)
                score = _asteroidTable[i].score;
        }

        Debug.Assert(score != 0, "The given type was not present in the score sheet");

        return score;
    }
}
