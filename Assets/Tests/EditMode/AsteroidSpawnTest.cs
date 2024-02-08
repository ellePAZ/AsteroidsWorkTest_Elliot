using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Enemies;
using NUnit.Framework;
using Spawning;
using UnityEngine;
using UnityEngine.TestTools;

public class AsteroidSpawnTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void SpawnAsteroid()
    {
        // Use the Assert class to test conditions

        AsteroidSpawner spawner = new AsteroidSpawner(null, null);

        var pos = Vector3.zero;
        pos = spawner.GetNewSpawnPosition(pos);
        Assert.IsTrue(Mathf.RoundToInt(pos.magnitude) >= spawner.noSpawnRadius, $"Pos magnitude = {pos.magnitude}\nNo spawn radius = {spawner.noSpawnRadius}");

        for (int i = 0; i < 100; i++)
        {
            pos = spawner.GetNewSpawnPosition(null);
            Assert.IsTrue(Mathf.RoundToInt(pos.magnitude) >= spawner.noSpawnRadius);
        }

        // Create a new test that spawns a ship and checks that _ship isn't null
    }
}
