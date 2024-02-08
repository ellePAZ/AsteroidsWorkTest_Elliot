using Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Asteroids/WaveData")]
public class WaveData : ScriptableObject
{
    public byte initialAsteroidCount;
    public int asteroidIncreaseInterval;
    public float initialShipSpawnInterval;
    public float shipSpawnIntervalDecrease;
}
