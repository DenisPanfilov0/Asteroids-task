using System;

namespace Code.App.Services
{
    public interface ICollisionService
    {
        event Action<int> OnEnemyHitByBullet;
        event Action<int> OnEnemyHitByLaser;
        void HandleBulletCollision(int enemyId);
        void HandleLaserCollision(int enemyId);
        void HandlePlayerCollision();
    }
}