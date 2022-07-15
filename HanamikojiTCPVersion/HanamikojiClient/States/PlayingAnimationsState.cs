using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiClient.States
{
    public class PlayingAnimationsState : AbstractClientState
    {
        public PlayingAnimationsState(TcpGameClient client) : base(client) { }
        private DateTime _startTime;
        private TimeSpan _breakDuration;

        public override void EnterState()
        {
            Console.WriteLine("Entered State: PlayingAnimationsState");
            _startTime = DateTime.UtcNow;
            _breakDuration = TimeSpan.FromMilliseconds(3000);
        }
        
        public override AbstractClientState? DoWork()
        {
            if (DateTime.UtcNow - _startTime < _breakDuration) return null;

            return new AwaitingPacketState(_client);
        }

        public override void ExitState()
        {
            _client.SendToServer(PacketCommandEnum.Ready);
        }
    }
}
