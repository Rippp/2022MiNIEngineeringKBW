using System.Net.Sockets;

namespace Server;
public class Game
{
    public Guid GameIdentifier = Guid.NewGuid();
    public bool MissingPlayer => _playerOneTcpClient is null || _playerTwoTcpClient is null;

    public int RequiredPlayers = 2;

    private TcpGameServer _server;
    private TcpClient? _playerOneTcpClient;
    private TcpClient? _playerTwoTcpClient;

    public Game(TcpGameServer server)
    {
        _server = server;
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
        while (true)
        {
            Console.WriteLine($"Playing game {GameIdentifier}!");
            Thread.Sleep(5000);
        }
    }
}
