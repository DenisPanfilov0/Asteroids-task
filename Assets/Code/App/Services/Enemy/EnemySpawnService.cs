using System;
using System.Collections.Generic;
using Code.App.Services.PlayerShip;
using UnityEngine;

namespace Code.App.Services.Enemy
{
    public interface IEnemySpawnService
    {
        void UpdateTimer(float deltaTime);
        event Action<int> OnSpawnAsteroid;
        event Action<int> OnSpawnFlyingSaucer;
        void CleanUp();
    }

    public class EnemySpawnService : IEnemySpawnService
    {
        public event Action<int> OnSpawnAsteroid;
        public event Action<int> OnSpawnFlyingSaucer;

        private readonly List<int> _asteroidIds = new List<int>();
        private readonly List<int> _flyingSaucerIds = new List<int>();
        private readonly IShipPositionService _shipPositionService;
        private readonly IIdGeneratorService _idGeneratorService;
        private IEnemyMovementService _enemyMovementService;
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

        public EnemySpawnService(IShipPositionService shipPositionService, IIdGeneratorService idGeneratorService,
            IEnemyMovementService enemyMovementService)
        {
            _shipPositionService = shipPositionService;
            _idGeneratorService = idGeneratorService;
            _enemyMovementService = enemyMovementService;

            Initialize();
        }

        private void Initialize()
        {
            _enemyMovementService.OnEnemyRemoved += RemoveEnemy;
            _enemyMovementService.OnSpawnSmallAsteroid += RegisterSmallAsteroid;
        }

        public void UpdateTimer(float deltaTime)
        {
            if (_enemyMovementService == null)
            {
                return;
            }

            _asteroidTimer += deltaTime;
            
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
            
            _flyingSaucerTimer += deltaTime;
            
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
            _asteroidTimer = 0f;
            _flyingSaucerTimer = 0f;
            _nextAsteroidSpawnTime = MinAsteroidSpawnInterval;
            _nextFlyingSaucerSpawnTime = MinFlyingSaucerSpawnInterval;
        }

        private int GenerateId()
        {
            return _idGeneratorService.GenerateId();
        }

        private void RemoveEnemy(int id, bool isAsteroid)
        {
            if (isAsteroid)
            {
                _asteroidIds.Remove(id);
                
                if (_asteroidIds.Count < MaxAsteroids - 1)
                {
                    _asteroidTimer = Mathf.Max(_asteroidTimer, _nextAsteroidSpawnTime - 0.5f);
                }
            }
            else
            {
                _flyingSaucerIds.Remove(id);
            }
        }

        private void RegisterSmallAsteroid(int id, Vector2 position, Vector2 direction, float speed)
        {
            if (_asteroidIds.Contains(id))
            {
                return;
            }

            if (_asteroidIds.Count >= MaxAsteroids)
            {
                return;
            }

            _asteroidIds.Add(id);
            _enemyMovementService.AddEnemy(id, position, direction, speed, true, true);
            OnSpawnAsteroid?.Invoke(id);
        }

        private void SpawnEnemy(int id, bool isAsteroid)
        {
            var screen = _shipPositionService.GetDataModel();
            float screenWidth = screen.ScreenWidth;
            float screenHeight = screen.ScreenHeight;
            Vector2 center = new Vector2(screenWidth / 2f, screenHeight / 2f);

            int side = UnityEngine.Random.Range(0, 4);
            Vector2 spawnPosition;
            Vector2 direction;

            switch (side)
            {
                case 0:
                    spawnPosition = new Vector2(-10f, UnityEngine.Random.Range(0f, screenHeight));
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.right;
                    break;
                case 1:
                    spawnPosition = new Vector2(screenWidth + 10f, UnityEngine.Random.Range(0f, screenHeight));
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.left;
                    break;
                case 2:
                    spawnPosition = new Vector2(UnityEngine.Random.Range(0f, screenWidth), screenHeight + 10f);
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.down;
                    break;
                case 3:
                    spawnPosition = new Vector2(UnityEngine.Random.Range(0f, screenWidth), -10f);
                    direction = isAsteroid ? DirectionToCenter(spawnPosition, center) : Vector2.up;
                    break;
                default:
                    spawnPosition = Vector2.zero;
                    direction = Vector2.right;
                    break;
            }

            float speed = isAsteroid
                ? UnityEngine.Random.Range(20f, 40f)
                : UnityEngine.Random.Range(32f, 48f);

            _enemyMovementService.AddEnemy(id, spawnPosition, direction, speed, isAsteroid, false);
            
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