using Code.App.Services;
using Code.App.Services.Enemy;
using Code.App.Services.PlayerShip;
using Code.Infrastructure.Loading;
using Code.Infrastructure.Services.AssetProvider;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.StaticData;
using Code.Infrastructure.WindowsService;
using Code.Progress.Provider;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, ICoroutineRunner, IInitializable
    {
        public override void InstallBindings()
        {
            BindInfrastructureServices();
            BindCommonServices();
            BindGameplayScreenServices();
            BindStateMachine();
            BindStateFactory();
            BindGameStates();
            BindProgressServices();
        }

        private void BindStateMachine()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
        }

        private void BindStateFactory()
        {
            Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();
        }

        private void BindGameStates()
        {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameLoopState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameLoseState>().AsSingle();
        }
        
        private void BindProgressServices()
        {
            Container.Bind<IProgressProvider>().To<ProgressProvider>().AsSingle();
        }
        
        private void BindGameplayScreenServices()
        {
            Container.Bind<IShipPositionService>().To<ShipPositionService>().AsSingle();
            Container.Bind<IShipMovementService>().To<ShipMovementService>().AsSingle();
            Container.Bind<IEnemySpawnService>().To<EnemySpawnService>().AsSingle();
            Container.Bind<IEnemyMovementService>().To<EnemyMovementService>().AsSingle();
            Container.Bind<IBulletService>().To<BulletService>().AsSingle();
            Container.Bind<ICollisionService>().To<CollisionService>().AsSingle();
            Container.Bind<IGameStateService>().To<GameStateService>().AsSingle();
            Container.Bind<IIdGeneratorService>().To<IdGeneratorService>().AsSingle();
        }

        private void BindInfrastructureServices()
        {
            Container.BindInterfacesTo<BootstrapInstaller>().FromInstance(this).AsSingle();
            Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
            Container.Bind<IWindowFactory>().To<WindowFactory>().AsSingle();
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
        }

        private void BindCommonServices()
        {
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
        }

        public void Initialize()
        {
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
        }
    }
}