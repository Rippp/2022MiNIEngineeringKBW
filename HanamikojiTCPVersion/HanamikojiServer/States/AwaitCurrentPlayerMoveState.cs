using CommonResources;
using CommonResources.Game;
using CommonResources.Network;

namespace HanamikojiServer.States
{
    public class AwaitCurrentPlayerMoveState : AbstractServerState
    {
        public AwaitCurrentPlayerMoveState(HanamikojiGame game) : base(game) { }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: AwaitCurrentPlayerMoveState");
            _game.SendToCurrentPlayer(PacketCommandEnum.MakeMove);
        }

        public override AbstractServerState DoWork()
        {
            var currentPlayerPacket = _game.ReadFromCurrentPlayer().GetAwaiter().GetResult();

            if (currentPlayerPacket != null && currentPlayerPacket.Command == PacketCommandEnum.PlayerMove)
            {
                var moveData = MoveData.DeserializeFromJson(currentPlayerPacket.Message);
                ConsoleWrapper.WriteInfo(moveData.MoveType.ToString());
                ConsoleWrapper.ConsoleWriteCards(moveData.GiftCards, "Wybrane karty");
                return new ValidateCurrentPlayerMoveState(_game, moveData);
            }

            return null;
        }

        public override void ExitState()
        {

        }
    }
}
