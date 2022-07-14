using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    public class BeginRoundState : AbstractServerState
    {
        public BeginRoundState(HanamikojiGame game) : base(game) { }
        private bool _currentPlayerReady = false;
        private bool _otherPlayerReady = false;

        public override void EnterState()
        {
            Console.WriteLine("Entered State: BeginRoundState");
            _game.StartNewRound();
            _game.SendGameDataToPlayers();
        }

        public override AbstractServerState? DoWork()
        { 
            Packet currentPlayerPacket = null;
            Packet otherPlayerPacket = null;
            
            if (!_currentPlayerReady)
                currentPlayerPacket = _game.ReadFromCurrentPlayer().GetAwaiter().GetResult();

            if(!_otherPlayerReady)
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
