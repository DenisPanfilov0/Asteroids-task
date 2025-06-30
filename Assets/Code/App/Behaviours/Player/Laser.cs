using Code.App.Services.Interfaces;
using UnityEngine;

namespace Code.App.Behaviours.Player
{
    public class Laser : MonoBehaviour
    {
        private const float MAX_LIFETIME = 1f;
        
        private IBulletService _bulletService;
        private int _id;
        private float _lifetime;

        public void Initialize(int id, IBulletService bulletService)
        {
            _id = id;
            _bulletService = bulletService;
        }

        private void Update()
        {
            _lifetime += Time.deltaTime;
            
            if (_lifetime >= MAX_LIFETIME)
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