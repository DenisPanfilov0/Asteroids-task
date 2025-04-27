using Code.App.Presenters;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class UIPresenterInitializer : IInitializable
    {
        private readonly GeneralStatisticsPresenter _generalStatisticsPresenter;
        private readonly ScoreDisplayPresenter _scoreDisplayPresenter;

        public UIPresenterInitializer(
            GeneralStatisticsPresenter generalStatisticsPresenter,
            ScoreDisplayPresenter scoreDisplayPresenter)
        {
            _generalStatisticsPresenter = generalStatisticsPresenter;
            _scoreDisplayPresenter = scoreDisplayPresenter;
        }

        public void Initialize()
        {
            _generalStatisticsPresenter.Initialize();
            _scoreDisplayPresenter.Initialize();
        }
    }
}