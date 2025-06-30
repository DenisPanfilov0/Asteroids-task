using Code.App.Services.Interfaces;
using Code.App.Services.Models;
using UnityEngine;

namespace Code.App.Behaviours.Player
{
    public class Bullet : MonoBehaviour
    {
        private const float MAX_LIFETIME = 5f;
        
        private IBulletService _bulletService;
        private Vector2 _direction;
        private int _id;
        private float _speed;
        private float _lifetime;

        public void Initialize(int id, BulletData bulletData, IBulletService bulletService)
        {
            _id = id;
            _bulletService = bulletService;
            _direction = bulletData.Direction;
            _speed = bulletData.Speed / 10;
        }

        private void Update()
        {
            Vector2 newPosition = (Vector2)transform.position + _direction * _speed * Time.deltaTime;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

            _lifetime += Time.deltaTime;
            if (_lifetime >= MAX_LIFETIME)
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