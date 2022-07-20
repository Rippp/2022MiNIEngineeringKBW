using CommonResources.Game;
using CommonResources.Network;

namespace HanamikojiClient.States
{
    public class AwaitingPacketState : AbstractClientState
    {
        public AwaitingPacketState(TcpGameClient client) : base(client) { }

        public override void EnterState() 
        {
            Console.WriteLine("Entered State: AwaitingPacketState");
        }

        public override AbstractClientState? DoWork()
        {
            var serverPacket = _client.ReadFromServer().GetAwaiter().GetResult();

            if (serverPacket != null)
            {
                _client.DisplayPacket(serverPacket);
                switch (serverPacket.Command)
                {
                    case PacketCommandEnum.GameState:
                        _client.ProcessGameData(GameData.DeserializeFromJson(serverPacket.Message));
                        return new PlayingAnimationsState(_client);

                    case PacketCommandEnum.MakeMove:
                        return new AwaitingUserMakeMoveState(_client);
                }
            }

            return null;
        }

        public override void ExitState(){ }
    }
}
