using CommonResources;
using CommonResources.Game;
using CommonResources.Network;

namespace HanamikojiServer.States
{
    public class AwaitCurrentPlayerMoveState : AbstractServerState
    {
        private readonly MoveData? _ongoingTradeMove;
        public AwaitCurrentPlayerMoveState(HanamikojiGame game, MoveData? ongoingTradeMove = null) : base(game) 
        {
            _ongoingTradeMove = ongoingTradeMove;
        }

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

                if (_ongoingTradeMove != null) 
                    moveData.TradeMoveGiftCards = _ongoingTradeMove.GiftCards;
                
                return new ValidateCurrentPlayerMoveState(_game, moveData);
            }

            return null;
        }

        public override void ExitState()
        {

        }
    }
}
