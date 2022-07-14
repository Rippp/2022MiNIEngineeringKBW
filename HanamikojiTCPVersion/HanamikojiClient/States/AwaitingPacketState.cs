using CommonResources.Network;

namespace HanamikojiClient.States
{
    public class AwaitingPacketState : AbstractClientState
    {
        public AwaitingPacketState(TcpGameClient client) : base(client) { }

        public override void EnterState() 
        {
            Console.WriteLine("Entered State: AwaitingFroGameDataState");
        }

        public override AbstractClientState? DoWork()
        {
            var serverPacket = _client.ReadFromServer().GetAwaiter().GetResult();

            if (serverPacket != null && serverPacket.Command == PacketCommandEnum.GameData)
            {
                _client.DisplayPacket(serverPacket);
                return new PlayingAnimationsState(_client);
            }
            
            // jezeli dostaniemy MakeMove ...

            return null;
        }

        public override void ExitState(){ }
    }
}
