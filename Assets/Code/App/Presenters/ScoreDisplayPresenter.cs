using Code.App.Behaviours;
using Code.App.Models.Interfaces;
using Code.App.Services;
using Code.App.View;

namespace Code.App.Presenters
{
    public class ScoreDisplayPresenter
    {
        private readonly IGameStateModel _gameStateModel;
        private readonly ScoreDisplayView _view;

        public ScoreDisplayPresenter(IGameStateModel gameStateModel, ScoreDisplayView view)
        {
            _gameStateModel = gameStateModel;
            _view = view;
        }

        public void Initialize()
        {
            _view.SetScore(_gameStateModel.GetScore());
            _gameStateModel.OnScoreChanged += UpdateScore;
        }

        public void Cleanup()
        {
            _gameStateModel.OnScoreChanged -= UpdateScore;
        }

        private void UpdateScore(int score)
        {
            _view.SetScore(score);
        }
    }
}