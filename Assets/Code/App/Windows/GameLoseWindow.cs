using Code.App.Services;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.WindowsService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.App.Windows
{
    public class GameLoseWindow : BaseWindow
    {
        [SerializeField] private Button _restartLevel;
        [SerializeField] private TextMeshProUGUI _score;
        
        private IGameStateService _gameStateService;
        private IGameStateMachine _gameStateMachine;

        [Inject]
        public void Construct(IGameStateService gameStateService, IGameStateMachine gameStateMachine)
        {
            Id = WindowId.GameLoseWindow;
            
            _gameStateMachine = gameStateMachine;
            _gameStateService = gameStateService;
        }

        protected override void Initialize()
        {
            _score.text = $"Score: {_gameStateService.GetScore()}";
        }

        protected override void SubscribeUpdates()
        {
            _restartLevel.onClick.AddListener(RestartLevel);
        }

        protected override void UnsubscribeUpdates()
        {
            _restartLevel.onClick.RemoveListener(RestartLevel);
        }

        private void RestartLevel()
        {
            _gameStateMachine.Enter<GameLoopState>();
            Destroy(gameObject);
        }
    }
}