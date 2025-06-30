using System;
using Code.App.Services;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using Code.App.Configs;
using Code.App.Models;
using Code.App.Services.Interfaces;
using Code.App.Services.Models;
using Object = UnityEngine.Object;

namespace Code.App.Behaviours.Enemy
{
    public class EnemySpawner : IInitializable, IDisposable
    {
        private GameObject _asteroidPrefab;
        private GameObject _smallAsteroidPrefab;
        private GameObject _ufoEnemyPrefab;
        private Transform _enemyContainer;
        private IEnemySpawnService _enemySpawnService;
        private ICollisionService _collisionService;
        private Dictionary<int, GameObject> _activeEnemies = new Dictionary<int, GameObject>();
        private IPlayerShipModel _playerShipModel;

        [Inject]
        public void Construct(IEnemySpawnService enemySpawnService, ICollisionService collisionService, 
            IPlayerShipModel playerShipModel, EnemyConfigList enemyConfigList, Transform enemyContainer)
        {
            _enemySpawnService = enemySpawnService;
            _collisionService = collisionService;
            _playerShipModel = playerShipModel;
            _asteroidPrefab = enemyConfigList.AsteroidPrefab;
            _smallAsteroidPrefab = enemyConfigList.SmallAsteroidPrefab;
            _ufoEnemyPrefab = enemyConfigList.UfoEnemyPrefab;
            _enemyContainer = enemyContainer;
        }

        public void Initialize()
        {
            _enemySpawnService.OnSpawnAsteroid += SpawnAsteroid;
            _enemySpawnService.OnSpawnFlyingSaucer += SpawnFlyingSaucer;
            _enemySpawnService.OnEnemyRemoved += HandleEnemyRemoved;
        }
        
        public void Dispose()
        {
            if (_enemySpawnService != null)
            {
                _enemySpawnService.OnSpawnAsteroid -= SpawnAsteroid;
                _enemySpawnService.OnSpawnFlyingSaucer -= SpawnFlyingSaucer;
                _enemySpawnService.OnEnemyRemoved -= HandleEnemyRemoved;
            }
        }

        private void SpawnAsteroid(int id)
        {
            EnemyData data = _enemySpawnService.GetEnemyData(id);
            if (data == null)
            {
                return;
            }
            
            bool isSmallAsteroid = data.IsSmallAsteroid;
            
            GameObject prefab = isSmallAsteroid ? _smallAsteroidPrefab : _asteroidPrefab;
            if (prefab == null)
            {
                return;
            }

            Vector2 spawnPosition = data.Position;
            if (isSmallAsteroid && data.ParentAsteroidId != -1)
            {
                if (_activeEnemies.TryGetValue(data.ParentAsteroidId, out GameObject parentAsteroid) && parentAsteroid != null)
                {
                    spawnPosition = parentAsteroid.transform.position;
                }
            }

            SpawnEnemy(prefab, id, true, isSmallAsteroid, spawnPosition);
        }

        private void SpawnFlyingSaucer(int id)
        {
            if (_ufoEnemyPrefab == null)
            {
                return;
            }
            
            EnemyData data = _enemySpawnService.GetEnemyData(id);
            if (data == null)
            {
                return;
            }
            
            SpawnEnemy(_ufoEnemyPrefab, id, false, false, data.Position);
        }

        private void SpawnEnemy(GameObject prefab, int id, bool isAsteroid, bool isSmallAsteroid, Vector2 position)
        {
            EnemyData data = _enemySpawnService.GetEnemyData(id);
            if (data == null)
            {
                return;
            }

            GameObject enemy = Object.Instantiate(prefab, new Vector3(position.x, position.y, 0f), Quaternion.identity, _enemyContainer);

            if (isAsteroid)
            {
                var asteroid = enemy.GetComponent<Asteroid>();
                if (asteroid == null)
                {
                    Object.Destroy(enemy);
                    return;
                }
                
                asteroid.Initialize(id, data.Direction, data.Speed, _collisionService);
            }
            else
            {
                var saucer = enemy.GetComponent<UfoEnemy>();
                if (saucer == null)
                {
                    Object.Destroy(enemy);
                    return;
                }
                
                saucer.Initialize(id, data.Direction, data.Speed, _collisionService, _playerShipModel);
            }

            if (_activeEnemies.ContainsKey(id))
            {
                if (_activeEnemies[id] != null)
                {
                    Object.Destroy(_activeEnemies[id]);
                }
                
                _activeEnemies.Remove(id);
            }

            _activeEnemies[id] = enemy;
        }

        private void HandleEnemyRemoved(int id, bool isAsteroid)
        {
            if (_activeEnemies.TryGetValue(id, out GameObject enemy))
            {
                if (enemy != null)
                {
                    Object.Destroy(enemy);
                }
                
                _activeEnemies.Remove(id);
            }
        }
    }
}