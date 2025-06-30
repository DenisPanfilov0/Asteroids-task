using System;
using Code.App.Models;
using Code.App.View;
using R3;
using UnityEngine.SceneManagement;
using Zenject;

namespace Code.App.Presenters
{
    public class GameLosePresenter : IInitializable, IDisposable
    {
        private readonly IPlayerShipModel _playerShipModel;
        private readonly GameLoseWindowView _view;
        private readonly CompositeDisposable _disposables = new();

        public GameLosePresenter(
            GameLoseWindowView view,
            IPlayerShipModel playerShipModel)
        {
            _playerShipModel = playerShipModel;
            _view = view;
        }

        public void Initialize()
        {
            _view.SetScore(_playerShipModel.Score.Value);
            _playerShipModel.Score.Subscribe(score => _view.SetScore(score)).AddTo(_disposables);
            
            _view.OnRestartClicked += HandleRestart;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            
            _view.OnRestartClicked -= HandleRestart;
        }

        private void HandleRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public sealed class Factory : PlaceholderFactory<GameLoseWindowView, GameLosePresenter>
        {
        }
    }
}