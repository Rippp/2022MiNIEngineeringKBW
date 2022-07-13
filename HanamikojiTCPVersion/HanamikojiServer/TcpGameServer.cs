using CommonResources.Network;
using System.Net;
using System.Net.Sockets;

namespace HanamikojiServer;

public class TcpGameServer
{
    // Connection data
    public readonly int Port;
    public bool Running { get; private set; }

    // new clients
    private TcpListener _newConnectionListener;
    private List<TcpClient> _waitingLobby = new List<TcpClient>();

    // current games and clients
    private List<TcpClient> _connectedClients = new List<TcpClient>();
    private List<HanamikojiGame> _games = new List<HanamikojiGame>();
    private List<Thread> _gameThreads = new List<Thread>();
    private HanamikojiGame _nextGame;

    public TcpGameServer(int port)
    {
        Port = port;
        Running = false;

        _newConnectionListener = new TcpListener(IPAddress.Any, Port);
    }

    public async Task Run()
    {
        Running = true;
        _nextGame = new HanamikojiGame(this);

        _newConnectionListener.Start();
        Console.WriteLine("Waiting for incoming connections...");

        List<Task> newConnectionTasks = new List<Task>();

        while (Running)
        {
            if (_newConnectionListener.Pending())
            {
                newConnectionTasks.Add(NewConnectionHandler());
            }

            if (_waitingLobby.Count >= _nextGame.RequiredPlayers) // enough players to play next game
            {
                while (_nextGame.MissingPlayer)
                {
                    var firstPlayerInLobby = _waitingLobby.First();

                    _nextGame.AddPlayer(firstPlayerInLobby);
                    await PacketProcessing.SendPacket(firstPlayerInLobby.GetStream(),
                        new Packet("message", $"Joined to game {_nextGame.GameIdentifier}"));
                    _waitingLobby.RemoveAt(0);
                }

                Console.WriteLine("There is enough number of players. Let's start the game!");
                var gameThread = new Thread(new ThreadStart(_nextGame.Run));

                gameThread.Start();

                _games.Add(_nextGame);
                _gameThreads.Add(gameThread);

                _nextGame = new HanamikojiGame(this);
            }

            Parallel.ForEach(_waitingLobby, (clientInLobby) =>
            {
                if (IsClientDisconnected(clientInLobby))
                {
                    Console.WriteLine($"Client {clientInLobby.Client.RemoteEndPoint} has disconnected from the Game Server.");
                    HandleDisconnectedClient(clientInLobby);
                }
            });


            Thread.Sleep(10);
        }

        _newConnectionListener.Stop();
        Console.WriteLine("The server has been shut down.");
    }

    public static bool IsClientDisconnected(TcpClient client)
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

    public async Task NewConnectionHandler()
    {
        TcpClient newClient = await _newConnectionListener.AcceptTcpClientAsync();
        Console.WriteLine("New connection from {0}.", newClient.Client.RemoteEndPoint);

        _connectedClients.Add(newClient);
        _waitingLobby.Add(newClient);

        await PacketProcessing.SendPacket(newClient.GetStream(),
            new Packet("message", $"Welcome to the Game Server. Wait to join the game\n"));
    }

    public void HandleDisconnectedClient(TcpClient client)
    {
        // Remove from collections and free resources
        _connectedClients.Remove(client);
        _waitingLobby.Remove(client);

        // clean up client
        client.GetStream().Close();
        client.Close();
    }

    public void Shutdown()
    {
        if (Running)
        {
            Running = false;
            Console.WriteLine("Shutting down the Game(s) Server...");
        }
    }
}