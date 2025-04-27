using Code.App.Presenters;
using Code.App.View;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class LobbySceneInstaller : MonoInstaller
    {
        [SerializeField] private LobbyHUDView _lobbyHUDView;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LobbyHUDView>().FromInstance(_lobbyHUDView).AsSingle();
            Container.BindInterfacesAndSelfTo<LobbyHUDPresenter>().AsSingle().NonLazy();
        }
    }
}