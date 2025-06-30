using System;
using System.Collections.Generic;
using Code.App.Extensions;
using Code.App.Models;
using Code.App.Services.Interfaces;
using Code.App.Services.Models;
using UnityEngine;
using Zenject;

namespace Code.App.Services
{
    public class EnemySpawnService : IEnemySpawnService, IInitializable, IDisposable
    {
        private const int MAX_ASTEROIDS = 5;
        private const int MAX_FLYING_SAUCERS = 2;
        private const float MIN_ASTEROID_SPAWN_INTERVAL = 2f;
        private const float MAX_ASTEROID_SPAWN_INTERVAL = 5f;
        private const float MIN_FLYING_SAUCER_SPAWN_INTERVAL = 8f;
        private const float MAX_FLYING_SAUCER_SPAWN_INTERVAL = 10f;
        private const float SPAWN_OFFSET = 0.5f;

        public event Action<int> OnSpawnAsteroid;
        public event Action<int> OnSpawnFlyingSaucer;
        public event Action<int, bool> OnEnemyRemoved;

        private readonly ICollisionService _collisionService;
        private readonly IPlayerShipModel _playerShipModel;
        private readonly IIdGeneratorService _idGeneratorService;
        private readonly Camera _mainCamera;
        private readonly List<int> _asteroidIds = new List<int>();
        private readonly List<int> _smallAsteroidIds = new List<int>();
        private readonly List<int> _flyingSaucerIds = new List<int>();
        private readonly Dictionary<int, EnemyData> _enemies = new Dictionary<int, EnemyData>();
        private float _asteroidTimer;
        private float _flyingSaucerTimer;
        private float _nextAsteroidSpawnTime;
        private float _nextFlyingSaucerSpawnTime;


        public EnemySpawnService(IIdGeneratorService idGeneratorService, Camera mainCamera,
            ICollisionService collisionService, IPlayerShipModel playerShipModel)
        {
            _idGeneratorService = idGeneratorService;
            _mainCamera = mainCamera;

            _collisionService = collisionService;
            _playerShipModel = playerShipModel;
        }

        public void Initialize()
        {
            _collisionService.OnEnemyHitByBullet += HandleEnemyHit;
            _collisionService.OnEnemyHitByLaser += HandleEnemyHit;
        }

        public void Dispose()
        {
            _collisionService.OnEnemyHitByBullet -= HandleEnemyHit;
            _collisionService.OnEnemyHitByLaser -= HandleEnemyHit;

            _asteroidTimer = 0f;
            _flyingSaucerTimer = 0f;
            _nextAsteroidSpawnTime = MIN_ASTEROID_SPAWN_INTERVAL;
            _nextFlyingSaucerSpawnTime = MIN_FLYING_SAUCER_SPAWN_INTERVAL;
            _asteroidIds.Clear();
            _smallAsteroidIds.Clear();
            _flyingSaucerIds.Clear();
            _enemies.Clear();
        }

        public void Tick()
        {
            _asteroidTimer += Time.deltaTime;
            if (_asteroidIds.Count < MAX_ASTEROIDS && _asteroidTimer >= _nextAsteroidSpawnTime)
            {
                int newId = GenerateId();
                _asteroidIds.Add(newId);
                SpawnAsteroid(newId);
                ResetAsteroidTimer();
            }

            _flyingSaucerTimer += Time.deltaTime;
            if (_flyingSaucerTimer >= _nextFlyingSaucerSpawnTime && _flyingSaucerIds.Count < MAX_FLYING_SAUCERS)
            {
                int newId = GenerateId();
                _flyingSaucerIds.Add(newId);
                SpawnFlyingSaucer(newId);
                ResetFlyingSaucerSpawnTime();
            }
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

            RemoveEnemy(id, isAsteroid, isSmallAsteroid);
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
                    if (_asteroidIds.Count < MAX_ASTEROIDS - 1)
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
            _playerShipModel.HandleEnemyHit();
            OnEnemyRemoved?.Invoke(id, isAsteroid);
        }

        private void SpawnAsteroid(int id)
        {
            var (spawnPosition, direction) = CalculateSpawnPositionAndDirection(true);
            float speed = UnityEngine.Random.Range(2f, 4f);
            _enemies[id] = new EnemyData(spawnPosition, direction, speed, true, false);
            OnSpawnAsteroid?.Invoke(id);
        }

        private void SpawnFlyingSaucer(int id)
        {
            var (spawnPosition, direction) = CalculateSpawnPositionAndDirection(false);
            float speed = UnityEngine.Random.Range(1.5f, 2.5f);
            _enemies[id] = new EnemyData(spawnPosition, direction, speed, false, false);
            OnSpawnFlyingSaucer?.Invoke(id);
        }

        private void SpawnSmallAsteroid(Vector2 position, Vector2 direction, float speed, int parentAsteroidId)
        {
            int newId = GenerateId();
            _smallAsteroidIds.Add(newId);
            _enemies[newId] = new EnemyData(position, direction, speed, true, true, parentAsteroidId);
            OnSpawnAsteroid?.Invoke(newId);
        }

        private (Vector2 spawnPosition, Vector2 direction) CalculateSpawnPositionAndDirection(bool isAsteroid)
        {
            Bounds worldBounds = CameraExtensions.GetOrthographicBounds(_mainCamera);
            Vector2 center = worldBounds.center;
            int side = UnityEngine.Random.Range(0, 4);
            Vector2 spawnPosition;
            Vector2 direction;

            switch (side)
            {
                case 0: // Left
                    spawnPosition = new Vector2(worldBounds.min.x - SPAWN_OFFSET,
                        UnityEngine.Random.Range(worldBounds.min.y, worldBounds.max.y));
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.right;
                    break;
                case 1: // Right
                    spawnPosition = new Vector2(worldBounds.max.x + SPAWN_OFFSET,
                        UnityEngine.Random.Range(worldBounds.min.y, worldBounds.max.y));
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.left;
                    break;
                case 2: // Top
                    spawnPosition = new Vector2(UnityEngine.Random.Range(worldBounds.min.x, worldBounds.max.x),
                        worldBounds.max.y + SPAWN_OFFSET);
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.down;
                    break;
                case 3: // Bottom
                    spawnPosition = new Vector2(UnityEngine.Random.Range(worldBounds.min.x, worldBounds.max.x),
                        worldBounds.min.y - SPAWN_OFFSET);
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.up;
                    break;
                default:
                    spawnPosition = Vector2.zero;
                    direction = Vector2.right;
                    break;
            }

            return (spawnPosition, direction);
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

        private int GenerateId()
        {
            return _idGeneratorService.GenerateId();
        }

        private void ResetAsteroidTimer()
        {
            _asteroidTimer = 0f;
            _nextAsteroidSpawnTime = UnityEngine.Random.Range(MIN_ASTEROID_SPAWN_INTERVAL, MAX_ASTEROID_SPAWN_INTERVAL);
        }

        private void ResetFlyingSaucerSpawnTime()
        {
            _flyingSaucerTimer = 0f;
            _nextFlyingSaucerSpawnTime =
                UnityEngine.Random.Range(MIN_FLYING_SAUCER_SPAWN_INTERVAL, MAX_FLYING_SAUCER_SPAWN_INTERVAL);
        }
    }
}