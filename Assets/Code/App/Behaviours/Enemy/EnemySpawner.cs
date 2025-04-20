using Code.App.Services;
using Code.App.Services.Enemy;
using UnityEngine;
using Zenject;

namespace Code.App.Behaviours.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private GameObject _smallAsteroidPrefab;
        [SerializeField] private GameObject _ufoEnemyPrefab;
        [SerializeField] private Transform _enemyContainer;

        private IEnemySpawnService _enemySpawnService;
        private IEnemyMovementService _enemyMovementService;
        private ICollisionService _collisionService;

        [Inject]
        public void Construct(IEnemySpawnService enemySpawnService, IEnemyMovementService enemyMovementService,
            ICollisionService collisionService)
        {
            _collisionService = collisionService;
            _enemySpawnService = enemySpawnService;
            _enemyMovementService = enemyMovementService;
        }

        private void Start()
        {
            _enemySpawnService.OnSpawnAsteroid += SpawnAsteroid;
            _enemySpawnService.OnSpawnFlyingSaucer += SpawnFlyingSaucer;
        }

        private void OnDestroy()
        {
            _enemySpawnService.OnSpawnAsteroid -= SpawnAsteroid;
            _enemySpawnService.OnSpawnFlyingSaucer -= SpawnFlyingSaucer;
        }

        private void SpawnAsteroid(int id)
        {
            EnemyData data = _enemyMovementService.GetEnemyData(id);
            bool isSmallAsteroid = data != null && data.IsSmallAsteroid;
            SpawnEnemy(isSmallAsteroid ? _smallAsteroidPrefab : _asteroidPrefab, id, true, isSmallAsteroid);
        }

        private void SpawnFlyingSaucer(int id)
        {
            SpawnEnemy(_ufoEnemyPrefab, id, false, false);
        }

        private void SpawnEnemy(GameObject prefab, int id, bool isAsteroid, bool isSmallAsteroid)
        {
            EnemyData data = _enemyMovementService.GetEnemyData(id);
            
            if (data == null)
            {
                return;
            }

            GameObject enemy = Instantiate(prefab, new Vector3(data.Position.x, data.Position.y, 0f), Quaternion.identity, _enemyContainer);
            
            if (isAsteroid)
            {
                var asteroid = enemy.GetComponent<Asteroid>();
                asteroid.Initialize(id, _enemyMovementService, _collisionService);
            }
            else
            {
                var saucer = enemy.GetComponent<UfoEnemy>();
                saucer.Initialize(id, _enemyMovementService, _collisionService);
            }
        }
    }
}