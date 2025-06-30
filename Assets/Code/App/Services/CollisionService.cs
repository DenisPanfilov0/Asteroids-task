using System;
using Code.App.Models;

namespace Code.App.Services
{
    public class CollisionService : ICollisionService
    {
        public event Action<int> OnEnemyHitByBullet;
        public event Action<int> OnEnemyHitByLaser;
        
        private readonly IPlayerShipModel _playerShipModel;

        public CollisionService(IPlayerShipModel playerShipModel)
        {
            _playerShipModel = playerShipModel;
        }

        public void HandleBulletCollision(int enemyId)
        {
            OnEnemyHitByBullet?.Invoke(enemyId);
        }

        public void HandleLaserCollision(int enemyId)
        {
            OnEnemyHitByLaser?.Invoke(enemyId);
        }

        public void HandlePlayerCollision()
        {
            _playerShipModel.HandlePlayerCollision();
        }
    }
}