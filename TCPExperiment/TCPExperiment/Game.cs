using System.Net.Sockets;
using Infrastructure;
using Infrastructure.Entities;

namespace Server;
public class Game
{
    public Guid GameIdentifier = Guid.NewGuid();
    public bool MissingPlayer => _playerOneTcpClient is null || _playerTwoTcpClient is null;

    public int RequiredPlayers = 2;

    private TcpGameServer _server;
    private TcpClient? _playerOneTcpClient;
    private TcpClient? _playerTwoTcpClient;
    private bool _gameRunning;

    public Game(TcpGameServer server)
    {
        _server = server;
        _gameRunning = true;
    }

    public void AddPlayer(TcpClient tcpClient)
    {
        if (_playerOneTcpClient is null)
        {
            _playerOneTcpClient = tcpClient;
            Console.WriteLine($"Player one connected to game {GameIdentifier}!");
        }
        else if (_playerTwoTcpClient is null)
        {
            _playerTwoTcpClient = tcpClient;
            Console.WriteLine($"Player two connected to game {GameIdentifier}!");
        }
        else throw new Exception("There are already two players connected to this game!");
    }

    public void Run()
    {
        while (_gameRunning)
        {
            Console.WriteLine(value: $"Playing game {GameIdentifier}!");

            CheckIfPlayerDisconnected();

            SendGameState(_playerTwoTcpClient);

            Thread.Sleep(500);
        }

        if (_playerOneTcpClient?.Connected ?? false)
            _playerOneTcpClient?.GetStream().Close();
        _playerOneTcpClient?.Close();

        
        if(_playerTwoTcpClient?.Connected ?? false)
            _playerTwoTcpClient?.GetStream().Close();
        _playerTwoTcpClient?.Close();
    }

    private void SendGameState(TcpClient tcpClient)
    {
        PacketProcessing.SendPacket(tcpClient.GetStream(), new Packet(
            "PlayerData", new PlayerData().SerializeToJson())).GetAwaiter().GetResult();
    }

    private void CheckIfPlayerDisconnected()
    {
        if (CheckIfPlayerDisconnected(_playerOneTcpClient))
        {
            PacketProcessing.SendPacket(_playerTwoTcpClient.GetStream(), new Packet("error", "other player disconnected"))
                .GetAwaiter().GetResult();
            StopGame("Player one left the game");
        }

        if (CheckIfPlayerDisconnected(_playerTwoTcpClient))
        {
            PacketProcessing.SendPacket(_playerOneTcpClient.GetStream(), new Packet("error", "other player disconnected"))
                .GetAwaiter().GetResult();
            StopGame("Player two left the game");
        }
    }

    private void StopGame(string reason = "")
    {
        _gameRunning = false;
        Console.WriteLine($"[GAME {GameIdentifier}] : Stopping the Game because: {reason}");
    }

    private bool CheckIfPlayerDisconnected(TcpClient client)
    {
        try
        {
            var clientSocket = client.Client;
            return clientSocket.Poll(10 * 1000, SelectMode.SelectRead) && (clientSocket.Available == 0);
        }
        catch (SocketException)
        {
            // We got a socket error, assume it's disconnected
            return true;
        }
    }
}
