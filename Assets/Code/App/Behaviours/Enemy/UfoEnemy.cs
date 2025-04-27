using Code.App.Behaviours.Player;
using Code.App.Services;
using UnityEngine;

namespace Code.App.Behaviours.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class UfoEnemy : MonoBehaviour
    {
        private int _id;
        private ICollisionService _collisionService;
        private PlayerShip _playerShip;
        private Rigidbody2D _rb;
        private float _time;

        public void Initialize(int id, Vector2 position, Vector2 direction, float speed, PlayerShip playerShip, ICollisionService collisionService)
        {
            _id = id;
            _collisionService = collisionService;
            _playerShip = playerShip;
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0f;
            _rb.angularDamping = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.position = position;
            _rb.linearVelocity = direction * speed;
            _time = 0f;
        }

        private void Update()
        {
            Vector2 playerPosition = _playerShip.GetPosition();
            Vector2 toPlayer = (playerPosition - (Vector2)transform.position).normalized;
            Vector2 forward = toPlayer;

            _time += Time.deltaTime;
            Vector2 perpendicular = new Vector2(-forward.y, forward.x);
            float waveAmplitude = 50f;
            float waveFrequency = 2f;
            Vector2 wave = perpendicular * Mathf.Sin(_time * waveFrequency) * waveAmplitude;
            
            //пытался сделать красивое волновое движени, но не смог
            _rb.linearVelocity = forward * _rb.linearVelocity.magnitude/* + wave*/;
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