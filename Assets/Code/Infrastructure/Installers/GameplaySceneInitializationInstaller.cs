using Code.App.Behaviours.Enemy;
using Code.App.Behaviours.Player;
using Code.App.Configs;
using Code.App.Models;
using Code.App.Presenters;
using Code.App.Services;
using Code.App.Services.Interfaces;
using Code.App.View;
using Code.Infrastructure.WindowsService.Configs;
using Code.Infrastructure.WindowsService.MVP;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class GameplaySceneInitializationInstaller : MonoInstaller
    {
        [SerializeField] private GeneralStatisticsView _generalStatisticsView;
        [SerializeField] private ScoreDisplayView _scoreDisplayView;
        [SerializeField] private PlayerShip _playerShip;
        [SerializeField] private Camera _mainCamera;
        
        [SerializeField] private Transform _uiRoot;
        [SerializeField] private WindowsConfig _windowsConfig;
        [SerializeField] private Transform _enemySpawnContainer;
        [SerializeField] private EnemyConfigList _enemyConfig;

        public override void InstallBindings()
        {
            BindUIObjectAndPrefabs();
            BindPresenters();
            BindServices();
            BindFactory();
        }

        private void BindUIObjectAndPrefabs()
        {
            Container.BindInterfacesAndSelfTo<Camera>().FromInstance(_mainCamera).AsSingle();

            Container.BindInterfacesAndSelfTo<GeneralStatisticsView>()
                .FromComponentInNewPrefab(_generalStatisticsView)
                .UnderTransform(_uiRoot)
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<ScoreDisplayView>()
                .FromComponentInNewPrefab(_scoreDisplayView)
                .UnderTransform(_uiRoot)
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<PlayerShip>()
                .FromComponentInNewPrefab(_playerShip)
                .AsSingle()
                .NonLazy();
        }

        private void BindPresenters()
        {
            Container.BindInterfacesAndSelfTo<GeneralStatisticsPresenter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoreDisplayPresenter>().AsSingle().NonLazy();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<WindowCreator>().AsCached().WithArguments(_uiRoot, _windowsConfig).NonLazy();

            Container.BindInterfacesAndSelfTo<EnemySpawnService>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle()
                .WithArguments(_enemyConfig, _enemySpawnContainer).NonLazy();

            Container.BindInterfacesAndSelfTo<BulletService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PlayerShipModel>().AsSingle().NonLazy();
            Container.Bind<ICollisionService>().To<CollisionService>().AsSingle();
            Container.Bind<IIdGeneratorService>().To<IdGeneratorService>().AsSingle();
        }

        private void BindFactory()
        {
            Container
                .BindFactory<GameLoseWindowView, GameLosePresenter, GameLosePresenter.Factory>()
                .AsSingle()
                .NonLazy();
        }
    }
}