using CommonResources.Network;
using System.Net.Sockets;
using System.Text;

namespace HanamikojiClient;

public class TcpGameClient
{
    public readonly string ServerAddress;
    public readonly int Port;
    public bool Running { get; private set; }
    private TcpClient _server;

    private NetworkStream _msgStream = null;

    public TcpGameClient(string serverAddress, int port)
    {
        ServerAddress = serverAddress;
        Port = port;
        _server = new TcpClient();
        Running = false;
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
        while (Running)
        {
            _ = handleIncomingPackets();

            Thread.Sleep(10);
        }

        CleanupNetworkResources();
    }

    private async Task handleIncomingPackets()
    {
        try
        {
            if (_server.Available > 0)
            {
                await HandlePacket(PacketProcessing.ReceivePacket(_msgStream).GetAwaiter().GetResult());
            }
        }
        catch (Exception exception)
        {
            ConsoleWrapper.WriteError(exception.Message);
        }
    }

    private async Task HandlePacket(Packet packet)
    {

        ConsoleWrapper.WriteInfo("Received package!");
        Console.WriteLine(packet);

        if (packet.Command == "error")
            Disconnect();

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
