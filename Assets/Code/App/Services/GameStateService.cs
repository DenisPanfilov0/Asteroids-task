using System;
using Code.App.Services.Enemy;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;

namespace Code.App.Services
{
    public interface IGameStateService
    {
        event Action<int> OnScoreChanged;
        event Action<bool> OnPauseChanged;
        int GetScore();
        void CleanUp();
    }

    public class GameStateService : IGameStateService
    {
        public event Action<int> OnScoreChanged;
        public event Action<bool> OnPauseChanged;

        private readonly IGameStateMachine _stateMachine;
        private readonly ICollisionService _collisionService;
        private readonly IEnemyMovementService _enemyMovementService;
        private int _score;
        private bool _onPause;

        public GameStateService(
            IGameStateMachine stateMachine,
            ICollisionService collisionService,
            IEnemyMovementService enemyMovementService)
        {
            _stateMachine = stateMachine;
            _collisionService = collisionService;
            _enemyMovementService = enemyMovementService;

            _onPause = false;
            SubscribeUpdates();
        }

        private void SubscribeUpdates()
        {
            _collisionService.OnPlayerHit += HandlePlayerCollision;
            _enemyMovementService.OnEnemyRemoved += HandleEnemyDestroyed;
        }

        public int GetScore() => 
            _score;

        public void CleanUp()
        {
            _score = 0;
            _onPause = false;
            OnScoreChanged?.Invoke(_score);
            OnPauseChanged?.Invoke(_onPause);
        }

        private void HandlePlayerCollision(int value)
        {
            SetPauseState(true);
            _stateMachine.Enter<GameLoseState>();
            
            _enemyMovementService.CleanUp();
        }

        private void HandleEnemyDestroyed(int id, bool isAsteroid)
        {
            if (_onPause)
                return;
            
            _score++;
            OnScoreChanged?.Invoke(_score);
        }

        private void SetPauseState(bool isPaused)
        {
            _onPause = isPaused;
            OnPauseChanged?.Invoke(_onPause);
        }
    }
}