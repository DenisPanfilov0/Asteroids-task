using Code.Infrastructure.Loading;
using Zenject;

namespace Code.Infrastructure
{
    public class Bootstrapper : IInitializable
    {
        private const string LobbyScene = "LobbyScene";
        private readonly ISceneLoader _sceneLoader;

        public Bootstrapper(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public void Initialize()
        {
            _sceneLoader.LoadScene(LobbyScene);
        }
    }
}