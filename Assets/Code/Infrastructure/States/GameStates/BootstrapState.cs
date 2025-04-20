using Code.Infrastructure.Loading;
using Code.Infrastructure.States.StateInfrastructure;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.StaticData;

namespace Code.Infrastructure.States.GameStates
{
    public class BootstrapState : IState
    {
        private const string GameScene = "GameScene";
        private readonly IGameStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;
        private readonly IStaticDataService _staticDataService;

        public BootstrapState(IGameStateMachine stateMachine, ISceneLoader sceneLoader, IStaticDataService staticDataService)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _staticDataService = staticDataService;
        }
    
        public void Enter()
        {
            _staticDataService.LoadAll();
            
            _sceneLoader.LoadScene(GameScene, () => { _stateMachine.Enter<GameLoopState>(); });
        }

        public void Exit()
        {
      
        }
    }
}