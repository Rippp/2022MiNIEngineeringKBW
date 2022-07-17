using CommonResources.Network;

namespace HanamikojiClient.States
{
    internal class AwaitingServerMoveValidationState : AbstractClientState
    {
        public AwaitingServerMoveValidationState(TcpGameClient client) : base(client) { }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: AwaitingServerMoveValidationState");
        }

        public override AbstractClientState? DoWork()
        {
            var serverPacket = _client.ReadFromServer().GetAwaiter().GetResult();

            if (serverPacket != null)
            {
                _client.DisplayPacket(serverPacket);
                switch (serverPacket.Command)
                {
                    case PacketCommandEnum.MoveInvalid:
                        return new InvalidMoveState(_client, serverPacket.Message);
                    case PacketCommandEnum.MoveVerified:
                        return new ValidMoveState(_client);
                }
            }

            return null;
        }

        public override void ExitState() { }
    }
}
