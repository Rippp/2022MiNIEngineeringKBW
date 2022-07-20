using CommonResources;
using CommonResources.Game;
using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    public class AwaitOtherPlayerMoveState : AbstractServerState
    {
        private readonly MoveData _currentPlayerMoveData;
        public AwaitOtherPlayerMoveState(HanamikojiGame game, MoveData currentPlayerMoveData) : base(game) 
        {
            _currentPlayerMoveData = currentPlayerMoveData;
        }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: AwaitingOtherPlayerMoveState");
            _game.SendToOtherPlayer(PacketCommandEnum.MakeMove);
        }

        public override AbstractServerState DoWork()
        {
            var otherPlayerPacket = _game.ReadFromOtherPlayer().GetAwaiter().GetResult();

            if (otherPlayerPacket != null && otherPlayerPacket.Command == PacketCommandEnum.PlayerMove)
            {
                var otherPlayerMoveData = MoveData.DeserializeFromJson(otherPlayerPacket.Message);
                return new ValidateOtherPlayerMoveState(_game, _currentPlayerMoveData, otherPlayerMoveData);
            }

            return null;
        }

        public override void ExitState()
        {

        }
    }
}
