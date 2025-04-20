using System;
using System.Collections.Generic;
using System.Linq;
using Code.App.Services.PlayerShip;
using UnityEngine;

namespace Code.App.Services.Enemy
{
    public interface IEnemyMovementService
    {
        void AddEnemy(int id, Vector2 position, Vector2 direction, float speed, bool isAsteroid, bool isSmallAsteroid = false);
        void UpdateAllPositions(float deltaTime);
        EnemyData GetEnemyData(int id);
        event Action<int, Vector2> OnEnemyPositionChanged;
        event Action<int, bool> OnEnemyRemoved;
        event Action<int, Vector2, Vector2, float> OnSpawnSmallAsteroid;
        void CleanUp();
    }

    public class EnemyData
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Speed;
        public bool IsAsteroid;
        public bool IsSmallAsteroid;
        public float Time;

        public EnemyData(Vector2 position, Vector2 direction, float speed, bool isAsteroid, bool isSmallAsteroid)
        {
            Position = position;
            Direction = direction.normalized;
            Speed = speed;
            IsAsteroid = isAsteroid;
            IsSmallAsteroid = isSmallAsteroid;
            Time = 0f;
        }
    }

    public class EnemyMovementService : IEnemyMovementService
    {
        public event Action<int, Vector2> OnEnemyPositionChanged;
        public event Action<int, bool> OnEnemyRemoved;
        public event Action<int, Vector2, Vector2, float> OnSpawnSmallAsteroid;

        private readonly IShipPositionService _shipPositionService;
        private readonly ICollisionService _collisionService;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly Dictionary<int, EnemyData> _enemies = new Dictionary<int, EnemyData>();
        private readonly float _screenWidth;
        private readonly float _screenHeight;
        private const float BoundaryOffset = 10f;
        private const int MaxAsteroids = 5;

        public EnemyMovementService(
            IShipPositionService shipPositionService,
            ICollisionService collisionService,
            IIdGeneratorService idGeneratorService)
        {
            _shipPositionService = shipPositionService;
            _collisionService = collisionService;
            _idGeneratorService = idGeneratorService;
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;

            SubscribeUpdates();
        }

        private void SubscribeUpdates()
        {
            _collisionService.OnEnemyHitByBullet += HandleEnemyHitByBullet;
            _collisionService.OnEnemyHitByLaser += HandleEnemyHitByLaser;
        }

        public void AddEnemy(int id, Vector2 position, Vector2 direction, float speed, bool isAsteroid, bool isSmallAsteroid = false)
        {
            _enemies[id] = new EnemyData(position, direction, speed, isAsteroid, isSmallAsteroid);
            OnEnemyPositionChanged?.Invoke(id, position);
        }

        public void UpdateAllPositions(float deltaTime)
        {
            List<int> enemyIds = _enemies.Keys.ToList();
            List<int> enemiesToRemove = new List<int>();

            foreach (int id in enemyIds)
            {
                if (!_enemies.TryGetValue(id, out EnemyData data))
                    continue;

                if (data.IsAsteroid)
                {
                    data.Position += data.Direction * data.Speed * deltaTime;

                    if (IsOutOfBounds(data.Position))
                    {
                        enemiesToRemove.Add(id);
                        continue;
                    }
                }
                else
                {
                    Vector2 playerPosition = _shipPositionService.GetDataModel().ShipPosition;
                    Vector2 toPlayer = (playerPosition - data.Position).normalized;
                    Vector2 forward = toPlayer;

                    data.Time += deltaTime;
                    Vector2 perpendicular = new Vector2(-forward.y, forward.x);
                    float waveAmplitude = 50f;
                    float waveFrequency = 2f;
                    Vector2 wave = perpendicular * Mathf.Sin(data.Time * waveFrequency) * waveAmplitude;

                    data.Position += (forward * data.Speed + wave) * deltaTime;
                }

                _enemies[id] = data;
                OnEnemyPositionChanged?.Invoke(id, data.Position);
            }

            foreach (int id in enemiesToRemove)
            {
                if (_enemies.ContainsKey(id))
                {
                    OnEnemyRemoved?.Invoke(id, true);
                    _enemies.Remove(id);
                }
            }
        }

        public void CleanUp()
        {
            foreach (int id in _enemies.Keys.ToList())
            {
                OnEnemyRemoved?.Invoke(id, _enemies[id].IsAsteroid);
                _enemies.Remove(id);
            }
        }

        public EnemyData GetEnemyData(int id)
        {
            return _enemies.TryGetValue(id, out EnemyData data) ? data : null;
        }

        private void HandleEnemyHitByBullet(int enemyId)
        {
            HandleEnemyHit(enemyId);
        }

        private void HandleEnemyHitByLaser(int enemyId)
        {
            HandleEnemyHit(enemyId);
        }

        private void HandleEnemyHit(int enemyId)
        {
            if (_enemies.ContainsKey(enemyId))
            {
                EnemyData data = _enemies[enemyId];
                bool isAsteroid = data.IsAsteroid;
                bool isSmallAsteroid = data.IsSmallAsteroid;

                if (isAsteroid && !isSmallAsteroid)
                {
                    Vector2 position = data.Position;
                    float baseSpeed = data.Speed;
                    int currentAsteroidCount = _enemies.Values.Count(e => e.IsAsteroid);
                    int availableSlots = MaxAsteroids - (currentAsteroidCount - 1);

                    for (int i = 0; i < Math.Min(2, availableSlots); i++)
                    {
                        int newId = _idGeneratorService.GenerateId();
                        float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
                        Vector2 direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
                        float speed = baseSpeed * 2f;
                        OnSpawnSmallAsteroid?.Invoke(newId, position, direction, speed);
                    }
                }

                OnEnemyRemoved?.Invoke(enemyId, isAsteroid);
                _enemies.Remove(enemyId);
            }
        }

        private bool IsOutOfBounds(Vector2 position)
        {
            return position.x < -BoundaryOffset || position.x > _screenWidth + BoundaryOffset ||
                   position.y < -BoundaryOffset || position.y > _screenHeight + BoundaryOffset;
        }
    }
}