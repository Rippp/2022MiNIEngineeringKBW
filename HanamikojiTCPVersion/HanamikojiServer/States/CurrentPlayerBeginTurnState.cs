using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    public class CurrentPlayerBeginTurnState : AbstractServerState
    {
        private bool _currentPlayerReady = false;
        public CurrentPlayerBeginTurnState(HanamikojiGame game) : base(game) { }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: CurrentPlayerBeginTurnState");

            _game.DrawRandomCardsToCurrentPlayer(1);
            _game.SendGameDataToCurrentPlayer();
        }

        public override AbstractServerState DoWork()
        {
            Packet currentPlayerPacket = null;

            if (!_currentPlayerReady)
                currentPlayerPacket = _game.ReadFromCurrentPlayer().GetAwaiter().GetResult();

            if (currentPlayerPacket != null && currentPlayerPacket.Command == PacketCommandEnum.Ready)
                return new AwaitCurrentPlayerMoveState(_game);

            return null;
        }

        public override void ExitState()
        {
            
        }
    }
}
