using Code.Infrastructure.Loading;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class BootstrapEntryPoint : MonoBehaviour
    {
        private const string LOBBY_SCENE = "LobbyScene";
        
        private ISceneLoader _sceneLoader;

        [Inject]
        public void Construct(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void Start()
        {
            _sceneLoader.LoadScene(LOBBY_SCENE);
        }
    }
}