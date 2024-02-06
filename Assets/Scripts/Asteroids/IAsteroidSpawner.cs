namespace Spawning
{
    public interface IAsteroidSpawner
    {
        void Spawn(Enemies.AsteroidType asteroidType, UnityEngine.Vector3? position);
        void UnSpawn(object asteroidType);
    }
}