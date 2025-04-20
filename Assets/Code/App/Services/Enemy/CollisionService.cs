using System;

namespace Code.App.Services.Enemy
{
    public interface ICollisionService
    {
        event Action<int> OnEnemyHitByBullet;
        event Action<int> OnEnemyHitByLaser;
        event Action<int> OnPlayerHit;
        void HandleBulletCollision(int enemyId);
        void HandleLaserCollision(int enemyId);
        void HandlePlayerCollision(int enemyId);
    }

    public class CollisionService : ICollisionService
    {
        public event Action<int> OnEnemyHitByBullet;
        public event Action<int> OnEnemyHitByLaser;
        public event Action<int> OnPlayerHit;

        public void HandleBulletCollision(int enemyId) => 
            OnEnemyHitByBullet?.Invoke(enemyId);

        public void HandleLaserCollision(int enemyId) => 
            OnEnemyHitByLaser?.Invoke(enemyId);

        public void HandlePlayerCollision(int enemyId) => 
            OnPlayerHit?.Invoke(enemyId);
    }
}