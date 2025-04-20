using Code.App.Services;
using Code.App.Services.Enemy;
using Code.App.Services.PlayerShip;
using UnityEngine;
using Zenject;

namespace Code.App.Behaviours
{
    public class ServiceUpdate : MonoBehaviour
    {
        private IShipMovementService _shipMovementService;
        private IEnemySpawnService _enemySpawnService;
        private IEnemyMovementService _enemyMovementService;
        private IBulletService _bulletService;
        private IGameStateService _gameStateService;
        private bool _isPaused;

        [Inject]
        public void Construct(
            IShipMovementService shipMovementService,
            IEnemySpawnService enemySpawnService,
            IEnemyMovementService enemyMovementService,
            IBulletService bulletService,
            IGameStateService gameStateService)
        {
            _shipMovementService = shipMovementService;
            _enemySpawnService = enemySpawnService;
            _enemyMovementService = enemyMovementService;
            _bulletService = bulletService;
            _gameStateService = gameStateService;

            _isPaused = false;
        }

        private void Start()
        {
            _gameStateService.OnPauseChanged += HandlePauseChanged;
        }

        private void OnDestroy()
        {
            _gameStateService.OnPauseChanged -= HandlePauseChanged;
        }

        private void Update()
        {
            if (_isPaused)
            {
                return;
            }

            _shipMovementService.UpdateInput();
            _enemySpawnService.UpdateTimer(Time.deltaTime);
            _enemyMovementService.UpdateAllPositions(Time.deltaTime);
            _bulletService.UpdateInput();
            _bulletService.CheckBulletsBounds();
        }

        private void HandlePauseChanged(bool isPaused) => 
            _isPaused = isPaused;
    }
}