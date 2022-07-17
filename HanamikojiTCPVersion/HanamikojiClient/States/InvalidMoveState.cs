using CommonResources;
using CommonResources.Game;
using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiClient.States
{
    internal class InvalidMoveState : AbstractClientState
    {
        private string _errorMessage;
        public InvalidMoveState(TcpGameClient client, string errorMessage) : base(client) 
        {
            _errorMessage = errorMessage;
        }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: InvalidMoveState");
            ConsoleWrapper.WriteError(_errorMessage);
        }

        public override AbstractClientState? DoWork()
        {
            var serverPacket = _client.ReadFromServer().GetAwaiter().GetResult();

            if (serverPacket != null && serverPacket.Command == PacketCommandEnum.PlayerData)
            {
                _client.ProcessPlayerData(PlayerData.DeserializeFromJson(serverPacket.Message));
                return new AwaitingUserMakeMoveState(_client);
            }

            return null;
        }

        public override void ExitState() { }
    }
}
