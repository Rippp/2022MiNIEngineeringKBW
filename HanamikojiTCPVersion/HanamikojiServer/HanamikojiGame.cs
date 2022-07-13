using CommonResources.Game;
using CommonResources.Game.Constants;
using CommonResources.Network;
using HanamikojiServer.States;
using System.Net.Sockets;

namespace HanamikojiServer
{
    public class HanamikojiGame
    {
        public Guid GameIdentifier = Guid.NewGuid();
        public bool MissingPlayer => _playerOneTcpClient is null || _playerTwoTcpClient is null;
        public int RequiredPlayers = 2;

        private TcpGameServer _server;
        private TcpClient? _playerOneTcpClient;
        private TcpClient? _playerTwoTcpClient;

        private TcpClient? _currentPlayer;
        private TcpClient? _otherPlayer => _currentPlayer == _playerOneTcpClient ? _playerTwoTcpClient : _playerOneTcpClient;

        private bool _gameRunning;
        private AbstractServerState _currentState;

        private PlayerData _playerOneData;
        private PlayerData _playerTwoData;
        private PlayerData _currentPlayerData;
        private PlayerData _otherPlayerData => _currentPlayerData == _playerOneData ? _playerTwoData : _playerOneData;


        public List<GiftCard> CardDeck { get; set; }
        private Random _random;

        public HanamikojiGame(TcpGameServer server)
        {
            _server = server;
            _gameRunning = true;
            _playerOneData = new PlayerData();
            _playerTwoData = new PlayerData();
            _random = new Random();
            CardDeck = new List<GiftCard>();
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
            _currentPlayer = _playerOneTcpClient;
            _currentPlayerData = _playerOneData;
            ChangeState(new BeginRoundState(this));

            while (_gameRunning)
            {
                Console.WriteLine(value: $"Playing game {GameIdentifier}!");

                CheckIfPlayerDisconnected();
                ChangeState(_currentState.DoWork());

                Thread.Sleep(10);
            }


            DisconnectPlayer(_playerOneTcpClient);
            DisconnectPlayer(_playerTwoTcpClient);
        }

        private void DisconnectPlayer(TcpClient player)
        {
            if (player?.Connected ?? false)
                player?.GetStream().Close();
            player?.Close();
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

        public void SendToCurrentPlayer(string command, string message)
        {
            if (_currentPlayer == null) return;

            PacketProcessing.SendPacket(_currentPlayer.GetStream(), new Packet(command, message))
                .GetAwaiter().GetResult();
        }

        public void SendToOtherPlayer(string command, string message)
        {
            if (_otherPlayer == null) return;

            PacketProcessing.SendPacket(_otherPlayer.GetStream(), new Packet(command, message))
                .GetAwaiter().GetResult();
        }

        public void SwitchPlayer()
        {
            var isPlayerOneCurrent = _playerOneTcpClient == _currentPlayer;
            _currentPlayer = isPlayerOneCurrent ? _playerTwoTcpClient : _playerOneTcpClient;
            _currentPlayerData = isPlayerOneCurrent ? _playerTwoData : _playerOneData;
        }

        public void SendGameDataToPlayers()
        {
            SendToCurrentPlayer("GameData", _currentPlayerData.SerializeToJson());
            SendToOtherPlayer("GameData", _otherPlayerData.SerializeToJson());
        }

        public void StartNewRound()
        {
            CardDeck = new List<GiftCard>(GiftCardConstants.AllCards);
            _currentPlayerData.ClearData();
            _otherPlayerData.ClearData();
            _playerOneData.CardsOnHand.AddRange(GetRandomCards(6));
            _playerTwoData.CardsOnHand.AddRange(GetRandomCards(6));
        }

        public void ChangeState(AbstractServerState state)
        {
            if (state == null) return;

            if(_currentState != null)
                _currentState.ExitState();
            _currentState = state;
            _currentState.EnterState();
        }

        private List<GiftCard> GetRandomCards(int numberOfCards)
        {
            var cardsForPlayer = new List<GiftCard>();
            for (int i = 0; i < numberOfCards; i++)
            {
                var randomCardIndex = _random.Next(CardDeck.Count());
                cardsForPlayer.Add(CardDeck[randomCardIndex]);
                CardDeck.RemoveAt(randomCardIndex);
            }

            return cardsForPlayer;
        }
    }
}
