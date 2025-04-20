using Code.App.Services;
using Code.App.Services.PlayerShip;
using UnityEngine;

namespace Code.App.Behaviours.Weapon
{
    public class Laser : MonoBehaviour
    {
        private IBulletService _bulletService;
        private int _id;
        private float _lifetime = 0f;
        private const float MaxLifetime = 1f;

        public void Initialize(int id, IBulletService bulletService)
        {
            _id = id;
            _bulletService = bulletService;
        }

        private void Update()
        {
            _lifetime += Time.deltaTime;
            
            if (_lifetime >= MaxLifetime)
            {
                DestroySelf();
            }
        }

        private void OnDestroy()
        {
            _bulletService?.RemoveLaser(_id);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}