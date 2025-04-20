using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.App.Services.PlayerShip
{
    public interface IBulletService
    {
        void UpdateInput();
        void CheckBulletsBounds();
        void RemoveBullet(int id);
        void RemoveLaser(int id);
        event Action<int, BulletData> OnSpawnBullet;
        event Action<int, LaserData> OnSpawnLaser;
        event Action<int, float> OnLaserChargeChanged;
        int GetLaserCharges();
        float GetLaserChargeProgress();
        void CleanUp();
    }

    public class BulletData
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Speed;

        public BulletData(Vector2 position, Vector2 direction, float speed)
        {
            Position = position;
            Direction = direction.normalized;
            Speed = speed;
        }
    }
    
    public class LaserData
    {
        public Vector2 Position;

        public LaserData(Vector2 position)
        {
            Position = position;
        }
    }

    public class BulletService : IBulletService
    {
        public event Action<int, BulletData> OnSpawnBullet;
        public event Action<int, LaserData> OnSpawnLaser;
        public event Action<int, float> OnLaserChargeChanged;

        private readonly Dictionary<int, BulletData> _bullets = new Dictionary<int, BulletData>();
        private readonly Dictionary<int, LaserData> _lasers = new Dictionary<int, LaserData>();
        private readonly IShipPositionService _shipPositionService;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly float _screenWidth;
        private readonly float _screenHeight;
        private float _bulletFireCooldown = 0f;
        private float _laserFireCooldown = 0f;
        private int _laserCharges = 3;
        private float _laserChargeTimer = 0f;
        private const float BulletFireRate = 0.4f;
        private const float LaserFireRate = 1f;
        private const float LaserChargeTime = 10f;
        private const int MaxLaserCharges = 3;
        private const float BoundaryOffset = 10f;

        public BulletService(IShipPositionService shipPositionService, IIdGeneratorService idGeneratorService)
        {
            _shipPositionService = shipPositionService;
            _idGeneratorService = idGeneratorService;
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
        }

        public void UpdateInput()
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

        public void CheckBulletsBounds()
        {
            List<int> bulletsToRemove = new List<int>();
            List<int> lasersToRemove = new List<int>();

            foreach (int id in _bullets.Keys.ToList())
            {
                if (!_bullets.TryGetValue(id, out BulletData data))
                    continue;

                if (IsOutOfBounds(data.Position))
                {
                    bulletsToRemove.Add(id);
                }
            }

            foreach (int id in _lasers.Keys.ToList())
            {
                if (!_lasers.TryGetValue(id, out LaserData data))
                    continue;

                if (IsOutOfBounds(data.Position))
                {
                    lasersToRemove.Add(id);
                }
            }

            foreach (int id in bulletsToRemove)
            {
                RemoveBullet(id);
            }

            foreach (int id in lasersToRemove)
            {
                RemoveLaser(id);
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
            var shipData = _shipPositionService.GetDataModel();
            Vector2 spawnPosition = shipData.ShipPosition;
            float angleRad = shipData.RotationAngle.z * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));
            float speed = 120f;

            int bulletId = _idGeneratorService.GenerateId();
            var bulletData = new BulletData(spawnPosition, direction, speed);
            _bullets[bulletId] = bulletData;
            OnSpawnBullet?.Invoke(bulletId, bulletData);
        }

        private void SpawnLaser()
        {
            var shipData = _shipPositionService.GetDataModel();
            Vector2 spawnPosition = shipData.ShipPosition;

            int laserId = _idGeneratorService.GenerateId();
            var laserData = new LaserData(spawnPosition);
            _lasers[laserId] = laserData;
            OnSpawnLaser?.Invoke(laserId, laserData);
        }

        private bool IsOutOfBounds(Vector2 position)
        {
            return position.x < -BoundaryOffset || position.x > _screenWidth + BoundaryOffset ||
                   position.y < -BoundaryOffset || position.y > _screenHeight + BoundaryOffset;
        }
    }
}