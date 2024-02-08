using Enemies;
using NUnit.Framework;
using Spawning;
using UnityEngine;

public class AsteroidSpawnTest
{
    [Test]
    public void SpawnZone()
    {
        EnemySpawner spawner = new EnemySpawner(null, null);

        var pos = Vector3.zero;
        pos = spawner.GetNewSpawnPosition(EnemyType.LargeAsteroid, pos, true);
        Assert.IsTrue(Mathf.RoundToInt(pos.magnitude) >= spawner.noSpawnRadius, $"Pos magnitude = {pos.magnitude}\nNo spawn radius = {spawner.noSpawnRadius}");

        for (int i = 0; i < 100; i++)
        {
            pos = spawner.GetNewSpawnPosition(EnemyType.LargeAsteroid, null, true);
            Assert.IsTrue(Mathf.RoundToInt(pos.magnitude) >= spawner.noSpawnRadius, $"Pos magnitude = {pos.magnitude}\nNo spawn radius = {spawner.noSpawnRadius}");
        }
    }

    [Test]
    public void SaucerSpawnTest()
    {
        Vector2 minViewBounds = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 maxViewBounds = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));

        EnemySpawner spawner = new EnemySpawner(null, null);

        for (int i = 0; i < 100; i++)
        {
            var pos = Vector3.zero;
            spawner.GetPositionOutsideScreen(ref pos, Camera.main);

            bool spawnsOutsideOfView = pos.x <= minViewBounds.x + float.Epsilon || pos.x >= maxViewBounds.x - float.Epsilon || pos.y <= minViewBounds.y + float.Epsilon || pos.y >= maxViewBounds.y - float.Epsilon;
            Assert.IsTrue(spawnsOutsideOfView, $"Pos = {pos}\nMinView = {minViewBounds}\nMaxView = {maxViewBounds}");
        }
    }
}
