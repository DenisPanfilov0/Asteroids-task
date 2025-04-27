using Code.App.Behaviours.Player;
using Code.App.Services;
using UnityEngine;

namespace Code.App.Behaviours.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Asteroid : MonoBehaviour
    {
        private int _id;
        private ICollisionService _collisionService;
        private Rigidbody2D _rb;

        public void Initialize(int id, Vector2 position, Vector2 direction, float speed, bool isSmallAsteroid, ICollisionService collisionService)
        {
            _id = id;
            _collisionService = collisionService;
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0f;
            _rb.angularDamping = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.position = position;
            _rb.linearVelocity = direction * speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Bullet>())
            {
                Bullet bullet = other.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.DestroySelf();
                    _collisionService.HandleBulletCollision(_id);
                }
            }
            else if (other.GetComponent<Laser>())
            {
                Laser laser = other.GetComponent<Laser>();
                if (laser != null)
                {
                    _collisionService.HandleLaserCollision(_id);
                }
            }
            else if (other.GetComponent<PlayerShip>())
            {
                _collisionService.HandlePlayerCollision(_id);
            }
        }
    }
}