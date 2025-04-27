using System;
using Code.App.View;
using Code.Infrastructure.Loading;
using Zenject;

namespace Code.App.Presenters
{
    public class LobbyHUDPresenter : IInitializable, IDisposable
    {
        private const string GameScene = "GameScene";
        private readonly LobbyHUDView _lobbyHUDView;
        private readonly ISceneLoader _sceneLoader;

        public LobbyHUDPresenter(LobbyHUDView lobbyHUDView, ISceneLoader sceneLoader)
        {
            _lobbyHUDView = lobbyHUDView;
            _sceneLoader = sceneLoader;
        }

        public void Initialize()
        {
            _lobbyHUDView.OnStartGame += LoadGameplayScene;
        }

        public void Dispose()
        {
            _lobbyHUDView.OnStartGame -= LoadGameplayScene;
        }

        private void LoadGameplayScene()
        {
            _sceneLoader.LoadScene(GameScene, Dispose);
        }
    }
}