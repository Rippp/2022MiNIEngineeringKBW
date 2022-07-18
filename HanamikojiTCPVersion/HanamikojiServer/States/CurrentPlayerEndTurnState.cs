using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    internal class CurrentPlayerEndTurnState : AbstractServerState
    {
        private bool _currentPlayerReady = false;
        private bool _otherPlayerReady = false;
        public CurrentPlayerEndTurnState(HanamikojiGame game) : base(game) { }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: EndCurrentPlayerTurnState");
            _game.SwitchPlayer();
            _game.SendGameDataToPlayers();
        }

        public override AbstractServerState DoWork()
        {
            Packet currentPlayerPacket = null;
            Packet otherPlayerPacket = null;

            if (!_currentPlayerReady)
                currentPlayerPacket = _game.ReadFromCurrentPlayer().GetAwaiter().GetResult();

            if (!_otherPlayerReady)
                otherPlayerPacket = _game.ReadFromOtherPlayer().GetAwaiter().GetResult();

            if (currentPlayerPacket != null && currentPlayerPacket.Command == PacketCommandEnum.Ready)
                _currentPlayerReady = true;

            if (otherPlayerPacket != null && otherPlayerPacket.Command == PacketCommandEnum.Ready)
                _otherPlayerReady = true;

            return _currentPlayerReady && _otherPlayerReady ? new CurrentPlayerBeginTurnState(_game) : null;
        }

        public override void ExitState()
        {

        }
    }
}
