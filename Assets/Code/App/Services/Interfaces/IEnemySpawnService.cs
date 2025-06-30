using System;
using Code.App.Services.Models;
using Zenject;

namespace Code.App.Services.Interfaces
{
    public interface IEnemySpawnService : ITickable
    {
        event Action<int> OnSpawnAsteroid;
        event Action<int> OnSpawnFlyingSaucer;
        event Action<int, bool> OnEnemyRemoved;
        EnemyData GetEnemyData(int id);
    }
}