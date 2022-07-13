namespace HanamikojiServer.States
{
    public abstract class AbstractServerState
    {
        protected readonly HanamikojiGame _game;

        public AbstractServerState(HanamikojiGame game)
        {
            _game = game;
        }

        public abstract void EnterState();
        public abstract AbstractServerState DoWork();
        public abstract void ExitState();
    }
}