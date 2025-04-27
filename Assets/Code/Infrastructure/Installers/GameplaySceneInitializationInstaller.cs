using Code.App.Behaviours.Player;
using Code.App.Models;
using Code.App.Models.Interfaces;
using Code.App.Presenters;
using Code.App.Services;
using Code.App.Services.Interfaces;
using Code.App.View;
using Code.Infrastructure.WindowsService.MVP;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class GameplaySceneInitializationInstaller : MonoInstaller
    {
        [SerializeField] private GeneralStatisticsView generalStatisticsView;
        [SerializeField] private ScoreDisplayView scoreDisplayView;
        [SerializeField] private PlayerShip playerShip;
        [SerializeField] private WindowCreator windowCreator;
        [SerializeField] private GameLoseWindowView _gameLoseWindowView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GeneralStatisticsView>().FromInstance(generalStatisticsView).AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreDisplayView>().FromInstance(scoreDisplayView).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerShip>().FromInstance(playerShip).AsSingle();
            Container.BindInterfacesAndSelfTo<WindowCreator>().FromInstance(windowCreator).AsSingle();
            Container.BindInterfacesAndSelfTo<GameLoseWindowView>().FromInstance(_gameLoseWindowView).AsSingle();

            Container.BindInterfacesAndSelfTo<GeneralStatisticsPresenter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoreDisplayPresenter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameLosePresenter>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<UIPresenterInitializer>().AsSingle().NonLazy();

            Container.Bind<IEnemySpawnService>().To<EnemySpawnService>().AsSingle();
            Container.Bind<IBulletService>().To<BulletService>().AsSingle();
            Container.Bind<ICollisionService>().To<CollisionService>().AsSingle();
            Container.Bind<IGameStateModel>().To<GameStateModel>().AsSingle();
            Container.Bind<IIdGeneratorService>().To<IdGeneratorService>().AsSingle();

            var enemySpawnService = Container.Resolve<IEnemySpawnService>();
            var collisionService = Container.Resolve<ICollisionService>();
            enemySpawnService.Initialize(collisionService);
        }
    }
}