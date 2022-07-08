using System.Net.Sockets;
using System.Text;
using Infrastructure;

namespace Client;

public class TcpGameClient
{
    public readonly string ServerAddress;
    public readonly int Port;
    public bool Running { get; private set; }
    private TcpClient _client;

    private NetworkStream _msgStream = null;

    public TcpGameClient(string serverAddress, int port)
    {
        ServerAddress = serverAddress;
        Port = port;
        _client = new TcpClient();
        Running = false;
    }

    public void ConnectToServer()
    {
        try
        {
            _client.Connect(ServerAddress, Port);
        }
        catch (SocketException se)
        {
            ConsoleWrapper.WriteError(se.Message);
        }

        if (_client.Connected)
        {
            Running = true;
            ConsoleWrapper.WriteInfo($"Connected to the server at {_client.Client.RemoteEndPoint}");
            _msgStream = _client.GetStream();
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
    }

    private async Task handleIncomingPackets()
    {
        try
        {
            if (_client.Available > 0)
            {
                byte[] lengthBuffer = new byte[2];
                await _msgStream.ReadAsync(lengthBuffer, 0, 2);
                ushort packetByteSize = BitConverter.ToUInt16(lengthBuffer, 0);

                byte[] jsonBuffer = new byte[packetByteSize];
                await _msgStream.ReadAsync(jsonBuffer, 0, jsonBuffer.Length);

                string jsonString = Encoding.UTF8.GetString(jsonBuffer);
                Packet packet = Packet.DeserializeFromJson(jsonString);

                await HandlePacket(packet);
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
        // todo: send packet to disconnect
    }

    private void CleanupNetworkResources()
    {
        _msgStream?.Close();
        _msgStream = null;
        _client.Close();
    }
}
