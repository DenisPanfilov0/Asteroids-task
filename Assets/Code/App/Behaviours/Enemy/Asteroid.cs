using Code.App.Behaviours.Weapon;
using Code.App.Services;
using Code.App.Services.Enemy;
using UnityEngine;

namespace Code.App.Behaviours.Enemy
{
    public class Asteroid : MonoBehaviour
    {
        private int _id;
        private IEnemyMovementService _enemyMovementService;
        private ICollisionService _collisionService;

        public void Initialize(int id, IEnemyMovementService enemyMovementService, ICollisionService collisionService)
        {
            _id = id;
            _enemyMovementService = enemyMovementService;
            _collisionService = collisionService;

            _enemyMovementService.OnEnemyPositionChanged += HandlePositionChanged;
            _enemyMovementService.OnEnemyRemoved += HandleDestroy;
        }

        private void OnDestroy()
        {
            _enemyMovementService.OnEnemyPositionChanged -= HandlePositionChanged;
            _enemyMovementService.OnEnemyRemoved -= HandleDestroy;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Bullet"))
            {
                Bullet bullet = other.GetComponent<Bullet>();
                
                if (bullet != null)
                {
                    bullet.DestroySelf();
                    _collisionService.HandleBulletCollision(_id);
                }
            }
            else if (other.CompareTag("Laser"))
            {
                Laser laser = other.GetComponent<Laser>();
                
                if (laser != null)
                {
                    _collisionService.HandleLaserCollision(_id);
                }
            }
            else if (other.CompareTag("Player"))
            {
                _collisionService.HandlePlayerCollision(_id);
            }
        }

        private void HandlePositionChanged(int id, Vector2 position)
        {
            if (id == _id)
            {
                transform.position = new Vector3(position.x, position.y, transform.position.z);
            }
        }

        private void HandleDestroy(int id, bool isAsteroid)
        {
            if (id == _id) 
                Destroy(gameObject);
        }
    }
}