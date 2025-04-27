using System;
using System.Collections.Generic;
using Code.App.Extensions;
using Code.App.Services.Interfaces;
using Code.App.Services.Models;
using UnityEngine;
using Zenject;

namespace Code.App.Services
{
    public class EnemySpawnService : IEnemySpawnService
    {
        public event Action<int> OnSpawnAsteroid;
        public event Action<int> OnSpawnFlyingSaucer;
        public event Action<int, bool> OnEnemyRemoved;

        private readonly List<int> _asteroidIds = new List<int>();
        private readonly List<int> _smallAsteroidIds = new List<int>();
        private readonly List<int> _flyingSaucerIds = new List<int>();
        private readonly Dictionary<int, EnemyData> _enemies = new Dictionary<int, EnemyData>();
        private readonly IIdGeneratorService _idGeneratorService;
        private ICollisionService _collisionService;
        private float _asteroidTimer;
        private float _flyingSaucerTimer;
        private float _nextAsteroidSpawnTime;
        private float _nextFlyingSaucerSpawnTime;
        private const int MaxAsteroids = 5;
        private const int MaxFlyingSaucers = 2;
        private const float MinAsteroidSpawnInterval = 2f;
        private const float MaxAsteroidSpawnInterval = 5f;
        private const float MinFlyingSaucerSpawnInterval = 8f;
        private const float MaxFlyingSaucerSpawnInterval = 10f;

        public EnemySpawnService(IIdGeneratorService idGeneratorService, DiContainer container)
        {
            _idGeneratorService = idGeneratorService;
            
            container.Resolve<TickableManager>().Add(this);
        }

        public void Initialize(ICollisionService collisionService)
        {
            _collisionService = collisionService;
            _collisionService.OnEnemyHitByBullet += HandleEnemyHit;
            _collisionService.OnEnemyHitByLaser += HandleEnemyHit;
        }

        public void Tick()
        {
            _asteroidTimer += Time.deltaTime;
            if (_asteroidIds.Count < MaxAsteroids)
            {
                if (_asteroidTimer >= _nextAsteroidSpawnTime)
                {
                    int newId = GenerateId();
                    _asteroidIds.Add(newId);
                    SpawnEnemy(newId, true);
                    ResetAsteroidTimer();
                }
            }

            _flyingSaucerTimer += Time.deltaTime;
            if (_flyingSaucerTimer >= _nextFlyingSaucerSpawnTime && _flyingSaucerIds.Count < MaxFlyingSaucers)
            {
                int newId = GenerateId();
                _flyingSaucerIds.Add(newId);
                SpawnEnemy(newId, false);
                ResetFlyingSaucerSpawnTime();
            }
        }

        public void CleanUp()
        {
            if (_collisionService != null)
            {
                _collisionService.OnEnemyHitByBullet -= HandleEnemyHit;
                _collisionService.OnEnemyHitByLaser -= HandleEnemyHit;
            }

            _asteroidTimer = 0f;
            _flyingSaucerTimer = 0f;
            _nextAsteroidSpawnTime = MinAsteroidSpawnInterval;
            _nextFlyingSaucerSpawnTime = MinFlyingSaucerSpawnInterval;
            _asteroidIds.Clear();
            _smallAsteroidIds.Clear();
            _flyingSaucerIds.Clear();
            _enemies.Clear();
        }

        public EnemyData GetEnemyData(int id)
        {
            return _enemies.TryGetValue(id, out EnemyData data) ? data : null;
        }

        private void HandleEnemyHit(int id)
        {
            var data = GetEnemyData(id);
            if (data == null)
            {
                return;
            }

            bool isAsteroid = data.IsAsteroid;
            bool isSmallAsteroid = data.IsSmallAsteroid;

            if (isAsteroid && !isSmallAsteroid)
            {
                float baseSpeed = data.Speed;

                for (int i = 0; i < 2; i++)
                {
                    float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    Vector2 direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
                    float speed = baseSpeed * 2f;
                    SpawnSmallAsteroid(Vector2.zero, direction, speed, id);
                }
            }

            OnEnemyRemoved?.Invoke(id, isAsteroid);
            RemoveEnemy(id, isAsteroid, isSmallAsteroid);
        }

        private void SpawnSmallAsteroid(Vector2 position, Vector2 direction, float speed, int parentAsteroidId)
        {
            int newId = GenerateId();
            _smallAsteroidIds.Add(newId);
            _enemies[newId] = new EnemyData(position, direction, speed, true, true, parentAsteroidId);
            OnSpawnAsteroid?.Invoke(newId);
        }

        private void RemoveEnemy(int id, bool isAsteroid, bool isSmallAsteroid)
        {
            if (isAsteroid)
            {
                if (isSmallAsteroid)
                {
                    _smallAsteroidIds.Remove(id);
                }
                else
                {
                    _asteroidIds.Remove(id);
                    if (_asteroidIds.Count < MaxAsteroids - 1)
                    {
                        _asteroidTimer = Mathf.Max(_asteroidTimer, _nextAsteroidSpawnTime - 0.5f);
                    }
                }
            }
            else
            {
                _flyingSaucerIds.Remove(id);
            }

            _enemies.Remove(id);
        }

        private int GenerateId()
        {
            return _idGeneratorService.GenerateId();
        }

        private void SpawnEnemy(int id, bool isAsteroid)
        {
            Bounds worldBounds = Camera.main.GetOrthographicBounds();
            Vector2 center = worldBounds.center;
            float offset = 0.5f;

            int side = UnityEngine.Random.Range(0, 4);
            Vector2 spawnPosition;
            Vector2 direction;

            switch (side)
            {
                case 0:
                    spawnPosition = new Vector2(worldBounds.min.x - offset, UnityEngine.Random.Range(worldBounds.min.y, worldBounds.max.y));
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.right;
                    break;
                case 1:
                    spawnPosition = new Vector2(worldBounds.max.x + offset, UnityEngine.Random.Range(worldBounds.min.y, worldBounds.max.y));
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.left;
                    break;
                case 2:
                    spawnPosition = new Vector2(UnityEngine.Random.Range(worldBounds.min.x, worldBounds.max.x), worldBounds.max.y + offset);
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.down;
                    break;
                case 3:
                    spawnPosition = new Vector2(UnityEngine.Random.Range(worldBounds.min.x, worldBounds.max.x), worldBounds.min.y - offset);
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.up;
                    break;
                default:
                    spawnPosition = Vector2.zero;
                    direction = Vector2.right;
                    break;
            }

            float speed = isAsteroid
                ? UnityEngine.Random.Range(2f, 4f)
                : UnityEngine.Random.Range(1.5f, 2.5f);

            _enemies[id] = new EnemyData(spawnPosition, direction, speed, isAsteroid, false);

            if (isAsteroid)
                OnSpawnAsteroid?.Invoke(id);
            else
                OnSpawnFlyingSaucer?.Invoke(id);
        }

        private Vector2 DirectionToCenter(Vector2 position, Vector2 center)
        {
            Vector2 baseDirection = (center - position).normalized;
            float angle = UnityEngine.Random.Range(-30f, 30f) * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            return new Vector2(
                baseDirection.x * cos - baseDirection.y * sin,
                baseDirection.x * sin + baseDirection.y * cos
            ).normalized;
        }

        private void ResetAsteroidTimer()
        {
            _asteroidTimer = 0f;
            _nextAsteroidSpawnTime = UnityEngine.Random.Range(MinAsteroidSpawnInterval, MaxAsteroidSpawnInterval);
        }

        private void ResetFlyingSaucerSpawnTime()
        {
            _flyingSaucerTimer = 0f;
            _nextFlyingSaucerSpawnTime = UnityEngine.Random.Range(MinFlyingSaucerSpawnInterval, MaxFlyingSaucerSpawnInterval);
        }
    }
}