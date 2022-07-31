namespace HanamikojiServer.States
{
    public class CurrentPlayerBeginTurnState : AbstractServerState
    {
        public CurrentPlayerBeginTurnState(HanamikojiGame game) : base(game) { }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: CurrentPlayerBeginTurnState");

            _game.DrawRandomCardsToCurrentPlayer(1);
            _game.SendGameDataToPlayers(messageToCurrentPlayer: "It's your turn!", messageToOtherPlayer: "Awaiting other player move...");
        }

        public override AbstractServerState DoWork()
        {
            return new AwaitCurrentPlayerMoveState(_game);
        }

        public override void ExitState()
        {
            
        }
    }
}
