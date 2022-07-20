using CommonResources;
using CommonResources.Game;
using CommonResources.Network;

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

            if (serverPacket != null && serverPacket.Command == PacketCommandEnum.GameState)
            {
                _client.ProcessGameData(GameData.DeserializeFromJson(serverPacket.Message));
                return new AwaitingUserMakeMoveState(_client);
            }

            return null;
        }

        public override void ExitState() { }
    }
}
