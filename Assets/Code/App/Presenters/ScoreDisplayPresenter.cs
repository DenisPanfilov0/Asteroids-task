using System;
using Code.App.Models;
using Code.App.View;
using R3;
using Zenject;

namespace Code.App.Presenters
{
    public class ScoreDisplayPresenter : IInitializable, IDisposable
    {
        private readonly IPlayerShipModel _playerShipModel;
        private readonly ScoreDisplayView _view;
        private readonly CompositeDisposable _disposables = new();

        public ScoreDisplayPresenter(IPlayerShipModel playerShipModel, ScoreDisplayView view)
        {
            _playerShipModel = playerShipModel;
            _view = view;
        }

        public void Initialize()
        {
            _view.SetScore(_playerShipModel.Score.Value);
            _playerShipModel.Score.Subscribe(score => _view.SetScore(score)).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}