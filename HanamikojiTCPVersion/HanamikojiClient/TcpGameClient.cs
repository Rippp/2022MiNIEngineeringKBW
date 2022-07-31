using CommonResources;
using CommonResources.Game;
using CommonResources.Network;
using HanamikojiClient.States;
using System.Net.Sockets;
using System.Text;

namespace HanamikojiClient;

public class TcpGameClient
{
    public readonly string ServerAddress;
    public readonly int Port;
    public bool Running { get; private set; }
    private MoveData? _nextMoveData { get; set; }

    private TcpClient _server;
    private NetworkStream _msgStream = null;
    private AbstractClientState? _currentState = null;
    private GameData _gameData;

    public TcpGameClient(string serverAddress, int port)
    {
        ServerAddress = serverAddress;
        Port = port;
        _server = new TcpClient();
        Running = false;
        _gameData = new GameData(new PlayerData(), new PlayerData());
    }

    public void ConnectToServer()
    {
        try
        {
            _server.Connect(ServerAddress, Port);
        }
        catch (SocketException se)
        {
            ConsoleWrapper.WriteError(se.Message);
        }

        if (_server.Connected)
        {
            Running = true;
            ConsoleWrapper.WriteInfo($"Connected to the server at {_server.Client.RemoteEndPoint}");
            _msgStream = _server.GetStream();
        }
        else
        {
            CleanupNetworkResources();
            ConsoleWrapper.WriteError($"Wasn't able to connect to the server at {ServerAddress}:{Port}.");
        }
    }

    public void Run()
    {
        ChangeState(new AwaitingPacketState(this));

        while (Running)
        {
            if (IsDisconnect(_server))
                Disconnect();

            ChangeState(_currentState.DoWork());

            Thread.Sleep(10);
        }

        CleanupNetworkResources();
    }

    public GameData GetGameData() => _gameData;

    public bool IsMoveDataSet() => _nextMoveData != null;

    public MoveData GetMoveData()
    {
        if (_nextMoveData == null) throw new Exception("move data is not set");
        var moveData = _nextMoveData;
        _nextMoveData = null;
        return moveData; 
    }

    public void SetNextMoveData(MoveData nextMoveData)
    {
        _nextMoveData = nextMoveData;
    }

    private void ChangeState(AbstractClientState state)
    {
        if (state == null) return;

        if (_currentState != null)
            _currentState.ExitState();
        _currentState = state;
        _currentState.EnterState();
    }
    
    public async Task<Packet?> ReadFromServer()
    {
        try
        {
            if (_server.Available > 0)
            {
                var packet = await PacketProcessing.ReceivePacket(_msgStream);
                HandlePacket(packet);
                return packet;
            }
        }
        catch (Exception exception)
        {
            ConsoleWrapper.WriteError(exception.Message);
        }

        return null;
    }
    
    public void SendToServer(PacketCommandEnum command, string message="")
    {
        PacketProcessing.SendPacket(_msgStream, new Packet(command, message))
            .GetAwaiter().GetResult();
    }
    
    public void ProcessGameData(GameData gameData)
    {
        _gameData = gameData;
    }
   
    public void DisplayPacket(Packet packet)
    {
        Console.WriteLine(packet.ToString());
    }


    private async Task HandlePacket(Packet packet)
    {
        switch(packet.Command)
        {
            case PacketCommandEnum.Error:
                Disconnect();
                break;
        }

        // mock async action after package receive (for example wait for user input or etc);
        Func<Task> mockedHandlingAction = async () => await Task.Delay(1000);
        await mockedHandlingAction();
    }

    private static bool IsDisconnect(TcpClient client)
    {
        try
        {
            var socket = client.Client;
            return socket.Poll(10 * 1000, SelectMode.SelectRead) && (socket.Available == 0);
        }
        catch (SocketException)
        {
            return true;
        }
    }

    public void Disconnect()
    {
        Console.WriteLine("Disconnecting from the server...");
        Running = false;
    }

    private void CleanupNetworkResources()
    {
        _msgStream?.Close();
        _msgStream = null;
        _server.Close();
    }
}
