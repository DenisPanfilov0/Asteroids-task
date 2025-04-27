using System;
using System.Collections.Generic;
using System.Linq;
using Code.App.Behaviours.Player;
using Code.App.Extensions;
using Code.App.Services.Interfaces;
using Code.App.Services.Models;
using UnityEngine;
using Zenject;

namespace Code.App.Services
{
    public class BulletService : IBulletService
    {
        public event Action<int, BulletData> OnSpawnBullet;
        public event Action<int, LaserData> OnSpawnLaser;
        public event Action<int, float> OnLaserChargeChanged;

        private readonly Dictionary<int, BulletData> _bullets = new Dictionary<int, BulletData>();
        private readonly Dictionary<int, LaserData> _lasers = new Dictionary<int, LaserData>();
        private readonly PlayerShip _playerShip;
        private readonly IIdGeneratorService _idGeneratorService;
        private float _bulletFireCooldown = 0f;
        private float _laserFireCooldown = 0f;
        private int _laserCharges = 3;
        private float _laserChargeTimer = 0f;
        private const float BulletFireRate = 0.4f;
        private const float LaserFireRate = 1f;
        private const float LaserChargeTime = 10f;
        private const int MaxLaserCharges = 3;
        
        private readonly Bounds _worldBounds;
        private const float BoundaryOffset = 0.5f;

        public BulletService(PlayerShip playerShip, IIdGeneratorService idGeneratorService, DiContainer container)
        {
            _playerShip = playerShip;
            _idGeneratorService = idGeneratorService;
            _worldBounds = Camera.main.GetOrthographicBounds();
            
            container.Resolve<TickableManager>().Add(this);
        }

        public void Tick()
        {
            _bulletFireCooldown -= Time.deltaTime;
            _laserFireCooldown -= Time.deltaTime;

            if (Input.GetKey(KeyCode.Q) && _bulletFireCooldown <= 0f)
            {
                SpawnBullet();
                _bulletFireCooldown = BulletFireRate;
            }

            if (Input.GetKeyDown(KeyCode.E) && _laserCharges >= 1 && _laserFireCooldown <= 0f)
            {
                SpawnLaser();
                _laserCharges--;
                _laserFireCooldown = LaserFireRate;
                OnLaserChargeChanged?.Invoke(_laserCharges, GetLaserChargeProgress());
            }

            if (_laserCharges < MaxLaserCharges)
            {
                _laserChargeTimer += Time.deltaTime;
                
                if (_laserChargeTimer >= LaserChargeTime)
                {
                    _laserCharges++;
                    _laserChargeTimer = 0f;
                }
                
                OnLaserChargeChanged?.Invoke(_laserCharges, GetLaserChargeProgress());
            }
        }

        public void RemoveBullet(int id)
        {
            if (_bullets.ContainsKey(id))
            {
                _bullets.Remove(id);
            }
        }

        public void RemoveLaser(int id)
        {
            if (_lasers.ContainsKey(id))
            {
                _lasers.Remove(id);
            }
        }

        public int GetLaserCharges()
        {
            return _laserCharges;
        }

        public float GetLaserChargeProgress()
        {
            if (_laserCharges >= MaxLaserCharges)
                return 100f;
            
            return (_laserChargeTimer / LaserChargeTime) * 100f;
        }

        public void CleanUp()
        {
            _bullets.Clear();
            _lasers.Clear();
            _bulletFireCooldown = 0f;
            _laserFireCooldown = 0f;
            _laserCharges = MaxLaserCharges;
            _laserChargeTimer = 0f;
            OnLaserChargeChanged?.Invoke(_laserCharges, GetLaserChargeProgress());
        }

        private void SpawnBullet()
        {
            Vector2 spawnPosition = _playerShip.GetPosition();
            float angleRad = _playerShip.GetRotation() * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));
            float speed = 120f;

            int bulletId = _idGeneratorService.GenerateId();
            var bulletData = new BulletData(spawnPosition, direction, speed);
            _bullets[bulletId] = bulletData;
            OnSpawnBullet?.Invoke(bulletId, bulletData);
        }

        private void SpawnLaser()
        {
            Vector2 spawnPosition = _playerShip.GetPosition();

            int laserId = _idGeneratorService.GenerateId();
            var laserData = new LaserData(spawnPosition);
            _lasers[laserId] = laserData;
            OnSpawnLaser?.Invoke(laserId, laserData);
        }

        private bool IsOutOfBounds(Vector2 position)
        {
            float minX = _worldBounds.min.x - BoundaryOffset;
            float maxX = _worldBounds.max.x + BoundaryOffset;
            float minY = _worldBounds.min.y - BoundaryOffset;
            float maxY = _worldBounds.max.y + BoundaryOffset;

            return position.x < minX || position.x > maxX ||
                   position.y < minY || position.y > maxY;
        }
    }
}