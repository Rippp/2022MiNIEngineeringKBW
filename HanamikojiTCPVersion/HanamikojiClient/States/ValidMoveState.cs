using CommonResources;
using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiClient.States
{
    internal class ValidMoveState : AbstractClientState
    {
        public ValidMoveState(TcpGameClient client) : base(client) { }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: ValidMoveState");
            ConsoleWrapper.WriteInfo("Move has been made");
        }

        public override AbstractClientState? DoWork()
        {
            return new AwaitingPacketState(_client);
        }

        public override void ExitState() 
        {

        }
    }
}
