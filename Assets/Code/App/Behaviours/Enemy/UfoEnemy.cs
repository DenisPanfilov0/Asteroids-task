using Code.App.Behaviours.Player;
using Code.App.Models;
using Code.App.Services;
using UnityEngine;

namespace Code.App.Behaviours.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class UfoEnemy : MonoBehaviour
    {
        private int _id;
        private ICollisionService _collisionService;
        private IPlayerShipModel _playerShipModel;
        private Rigidbody2D _rb;

        public void Initialize(int id, Vector2 direction, float speed,
            ICollisionService collisionService, IPlayerShipModel playerShipModel)
        {
            _id = id;
            _collisionService = collisionService;
            _playerShipModel = playerShipModel;
            _rb = GetComponent<Rigidbody2D>();
            _rb.linearVelocity = direction * speed;
        }

        private void Update()
        {
            Vector2 playerPosition = _playerShipModel.GetPosition();
            Vector2 toPlayer = (playerPosition - (Vector2)transform.position).normalized;
            Vector2 forward = toPlayer;

            _rb.linearVelocity = forward * _rb.linearVelocity.magnitude;
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
                _collisionService.HandlePlayerCollision();
            }
        }
    }
}