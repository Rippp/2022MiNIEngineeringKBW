using CommonResources.Game;
using CommonResources.Game.Constants;
using CommonResources.Network;

namespace HanamikojiServer.States
{
    internal class CurrentPlayerEndTurnState : AbstractServerState
    {
        private PlayerData _currentPlayerData;
        private PlayerData _otherPlayerData;
        public CurrentPlayerEndTurnState(HanamikojiGame game) : base(game) { }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: CurrentPlayerEndTurnState");
            
            _currentPlayerData = _game.GetCurrentPlayerData();
            _otherPlayerData = _game.GetOtherPlayerData();
        }

        public override AbstractServerState DoWork()
        {
            if (!_currentPlayerData.IsAnyMoveAvailable() && !_otherPlayerData.IsAnyMoveAvailable())
                return new EndRoundState(_game);

            return new CurrentPlayerBeginTurnState(_game);
        }

        public override void ExitState()
        {
            _game.SwitchPlayer();
            _game.SendGameDataToPlayers();
        }
    }
}
