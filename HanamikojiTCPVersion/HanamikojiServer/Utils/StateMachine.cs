using HanamikojiServer.States;

namespace HanamikojiServer.Utils
{
    public class StateMachine
    {
        private AbstractServerState _currentState;

        public StateMachine(AbstractServerState initialState)
        {
            ChangeState(initialState);
        }

        public void Execute()
        {
            ChangeState(_currentState.DoWork());
        }

        private void ChangeState(AbstractServerState state)
        {
            if (state == null) return;

            if (_currentState != null)
                _currentState.ExitState();
            _currentState = state;
            _currentState.EnterState();
        }
    }
}
