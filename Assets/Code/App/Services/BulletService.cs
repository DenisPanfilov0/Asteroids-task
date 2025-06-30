using System;
using System.Collections.Generic;
using Code.App.Models;
using Code.App.Services.Interfaces;
using Code.App.Services.Models;
using UnityEngine;
using Zenject;

namespace Code.App.Services
{
    public class BulletService : IBulletService, ITickable, IDisposable
    {
        private const float BULLET_FIRE_RATE = 0.4f;
        private const float LASER_FIRE_RATE = 1f;
        private const float LASER_CHARGE_TIME = 10f;
        private const int MAX_LASER_CHARGES = 3;
        
        public event Action<int, BulletData> OnSpawnBullet;
        public event Action<int, LaserData> OnSpawnLaser;

        private IPlayerShipModel _playerShipModel;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly Dictionary<int, BulletData> _bullets = new Dictionary<int, BulletData>();
        private readonly Dictionary<int, LaserData> _lasers = new Dictionary<int, LaserData>();
        private float _bulletFireCooldown;
        private float _laserFireCooldown;
        private float _laserChargeTimer;
        private int _laserCharges = 3;

        public BulletService(IIdGeneratorService idGeneratorService)
        {
            _idGeneratorService = idGeneratorService;
        }

        public void Tick()
        {
            _bulletFireCooldown -= Time.deltaTime;
            _laserFireCooldown -= Time.deltaTime;

            if (Input.GetKey(KeyCode.Q) && _bulletFireCooldown <= 0f)
            {
                SpawnBullet();
                _bulletFireCooldown = BULLET_FIRE_RATE;
            }

            if (Input.GetKeyDown(KeyCode.E) && _laserCharges >= 1 && _laserFireCooldown <= 0f)
            {
                SpawnLaser();
                _laserCharges--;
                _laserFireCooldown = LASER_FIRE_RATE;
            }

            if (_laserCharges < MAX_LASER_CHARGES)
            {
                _laserChargeTimer += Time.deltaTime;
                
                if (_laserChargeTimer >= LASER_CHARGE_TIME)
                {
                    _laserCharges++;
                    _laserChargeTimer = 0f;
                }
                
                _playerShipModel.SetLaserCharge(_laserCharges, GetLaserChargeProgress());
            }
        }

        public void SetPlayerShip(IPlayerShipModel playerShipModel)
        {
            _playerShipModel = playerShipModel;
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
            if (_laserCharges >= MAX_LASER_CHARGES)
                return 100f;
            
            return (_laserChargeTimer / LASER_CHARGE_TIME) * 100f;
        }

        public void Dispose()
        {
            _bullets.Clear();
            _lasers.Clear();
            _bulletFireCooldown = 0f;
            _laserFireCooldown = 0f;
            _laserCharges = MAX_LASER_CHARGES;
            _laserChargeTimer = 0f;
        }

        private void SpawnBullet()
        {
            Vector2 spawnPosition = _playerShipModel.GetPosition();
            float angleRad = _playerShipModel.GetRotation() * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));
            float speed = 120f;

            int bulletId = _idGeneratorService.GenerateId();
            var bulletData = new BulletData(spawnPosition, direction, speed);
            _bullets[bulletId] = bulletData;
            OnSpawnBullet?.Invoke(bulletId, bulletData);
        }

        private void SpawnLaser()
        {
            Vector2 spawnPosition = _playerShipModel.GetPosition();

            int laserId = _idGeneratorService.GenerateId();
            var laserData = new LaserData(spawnPosition);
            _lasers[laserId] = laserData;
            OnSpawnLaser?.Invoke(laserId, laserData);
        }
    }
}