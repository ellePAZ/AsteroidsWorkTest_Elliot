namespace Spawning
{
    public interface IEnemySpawner
    {
        void Spawn(Enemies.EnemyType asteroidType, UnityEngine.Vector3? position, bool avoidMiddle);
        void UnSpawn(object asteroidType);

        public void Reset();
    }
}