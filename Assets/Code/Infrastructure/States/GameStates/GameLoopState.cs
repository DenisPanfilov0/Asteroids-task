using Code.Infrastructure.States.StateInfrastructure;

namespace Code.Infrastructure.States.GameStates
{
    public class GameLoopState : IState
    {
        //заглушка на моё усмотрение для определения стэйта игры, можно было и убрать его
        //или описать установку стартовых значений именно здесь
        public GameLoopState()
        {
        }
        
        public void Enter()
        {
        }

        public void Exit()
        {
        }
    }
}