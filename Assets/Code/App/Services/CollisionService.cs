using System;

namespace Code.App.Services
{
    public class CollisionService : ICollisionService
    {
        public event Action<int> OnEnemyHitByBullet;
        public event Action<int> OnEnemyHitByLaser;
        public event Action<int> OnPlayerHit;

        public void HandleBulletCollision(int enemyId)
        {
            OnEnemyHitByBullet?.Invoke(enemyId);
        }

        public void HandleLaserCollision(int enemyId)
        {
            OnEnemyHitByLaser?.Invoke(enemyId);
        }

        public void HandlePlayerCollision(int enemyId)
        {
            OnPlayerHit?.Invoke(enemyId);
        }
    }
}