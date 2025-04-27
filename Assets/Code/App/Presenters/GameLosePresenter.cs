using Code.App.Models.Interfaces;
using Code.App.Services;
using Code.App.View;
using Code.Infrastructure.WindowsService.MVP;
using UnityEngine.SceneManagement;
using Zenject;

namespace Code.App.Presenters
{
    public class GameLosePresenter : IInitializable
    {
        private readonly IGameStateModel _gameStateModel;
        private readonly GameLoseWindowView _view;

        public GameLosePresenter(
            IGameStateModel gameStateModel,
            GameLoseWindowView view)
        {
            _gameStateModel = gameStateModel;
            _view = view;
        }

        public void Initialize()
        {
            _gameStateModel.OnScoreChanged += UpdateScoreView;
            _view.SetScore(_gameStateModel.GetScore());
            _view.OnRestartClicked += HandleRestart;
        }

        private void UpdateScoreView(int score)
        {
            _view.SetScore(score);
        }

        public void Cleanup()
        {
            _view.OnRestartClicked -= HandleRestart;
        }

        private void HandleRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}