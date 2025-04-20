using Code.App.Services;
using Code.App.Services.Enemy;
using Code.App.Services.PlayerShip;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.WindowsService;

namespace Code.Infrastructure.States.GameStates
{
    public class GameLoseState : IState
    {
        private readonly IWindowService _windowService;
        private readonly IGameStateService _gameStateService;
        private readonly IEnemySpawnService _enemySpawnService;
        private readonly IEnemyMovementService _enemyMovementService;
        private readonly IBulletService _bulletService;
        private readonly IShipPositionService _shipPositionService;
        private readonly IShipMovementService _shipMovementService;

        public GameLoseState(
            IGameStateMachine stateMachine,
            IWindowService windowService,
            IGameStateService gameStateService,
            IEnemySpawnService enemySpawnService,
            IEnemyMovementService enemyMovementService,
            IBulletService bulletService,
            IShipPositionService shipPositionService,
            IShipMovementService shipMovementService)
        {
            _windowService = windowService;
            _gameStateService = gameStateService;
            _enemySpawnService = enemySpawnService;
            _enemyMovementService = enemyMovementService;
            _bulletService = bulletService;
            _shipPositionService = shipPositionService;
            _shipMovementService = shipMovementService;
        }

        public void Enter()
        {
            _windowService.Open(WindowId.GameLoseWindow);
        }

        public void Exit()
        {
            _gameStateService.CleanUp();
            _enemySpawnService.CleanUp();
            _enemyMovementService.CleanUp();
            _bulletService.CleanUp();
            _shipPositionService.CleanUp();
            _shipMovementService.CleanUp();
        }
    }
}