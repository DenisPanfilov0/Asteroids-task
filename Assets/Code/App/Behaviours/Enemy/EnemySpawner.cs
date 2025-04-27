using Code.App.Services;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using Code.App.Behaviours.Player;
using Code.App.Services.Interfaces;
using Code.App.Services.Models;

namespace Code.App.Behaviours.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _smallAsteroidPrefab;
        [SerializeField] private GameObject _ufoEnemyPrefab;
        [SerializeField] private Transform _enemyContainer;

        private IEnemySpawnService _enemySpawnService;
        private ICollisionService _collisionService;
        private PlayerShip _playerShip;
        private Dictionary<int, GameObject> _activeEnemies;

        [Inject]
        public void Construct(IEnemySpawnService enemySpawnService, ICollisionService collisionService, PlayerShip playerShip)
        {
            _enemySpawnService = enemySpawnService;
            _collisionService = collisionService;
            _playerShip = playerShip;
            _activeEnemies = new Dictionary<int, GameObject>();
        }

        private void Start()
        {
            _enemySpawnService.OnSpawnAsteroid += SpawnAsteroid;
            _enemySpawnService.OnSpawnFlyingSaucer += SpawnFlyingSaucer;
            _enemySpawnService.OnEnemyRemoved += HandleEnemyRemoved;
        }

        private void OnDestroy()
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

            GameObject enemy = Instantiate(prefab, new Vector3(position.x, position.y, 0f), Quaternion.identity, _enemyContainer);

            if (isAsteroid)
            {
                var asteroid = enemy.GetComponent<Asteroid>();
                if (asteroid == null)
                {
                    Destroy(enemy);
                    return;
                }
                asteroid.Initialize(id, position, data.Direction, data.Speed, isSmallAsteroid, _collisionService);
            }
            else
            {
                var saucer = enemy.GetComponent<UfoEnemy>();
                if (saucer == null)
                {
                    Destroy(enemy);
                    return;
                }
                saucer.Initialize(id, position, data.Direction, data.Speed, _playerShip, _collisionService);
            }

            if (_activeEnemies.ContainsKey(id))
            {
                if (_activeEnemies[id] != null)
                {
                    Destroy(_activeEnemies[id]);
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
                    Destroy(enemy);
                }
                _activeEnemies.Remove(id);
            }
        }
    }
}