using Code.App.Services;
using Code.App.Services.PlayerShip;
using UnityEngine;

namespace Code.App.Behaviours.Weapon
{
    public class Bullet : MonoBehaviour
    {
        private int _id;
        private IBulletService _bulletService;
        private Vector2 _direction;
        private float _speed;
        private float _lifetime = 0f;
        private const float MaxLifetime = 5f;

        public void Initialize(int id, BulletData bulletData, IBulletService bulletService)
        {
            _id = id;
            _bulletService = bulletService;
            _direction = bulletData.Direction;
            _speed = bulletData.Speed;
        }

        private void Update()
        {
            Vector2 newPosition = (Vector2)transform.position + _direction * _speed * Time.deltaTime;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

            _lifetime += Time.deltaTime;
            if (_lifetime >= MaxLifetime)
            {
                DestroySelf();
            }
        }

        private void OnDestroy()
        {
            _bulletService?.RemoveBullet(_id);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}