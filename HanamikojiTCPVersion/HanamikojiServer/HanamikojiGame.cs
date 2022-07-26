using CommonResources;
using CommonResources.Game;
using CommonResources.Game.Constants;
using CommonResources.Network;
using HanamikojiServer.States;
using System.Net.Sockets;
using HanamikojiServer.Utils;

namespace HanamikojiServer
{
    public class HanamikojiGame
    {
        public Guid GameIdentifier = Guid.NewGuid();
        public bool MissingPlayer => _playerOneTcpClient is null || _playerTwoTcpClient is null;
        public int RequiredPlayers = 2;
        private List<GiftCard> _cardDeck { get; set; }

        private TcpClient? _playerOneTcpClient;
        private TcpClient? _playerTwoTcpClient;
        private TcpClient? _currentPlayer;
        private TcpClient? _otherPlayer => _currentPlayer == _playerOneTcpClient ? _playerTwoTcpClient : _playerOneTcpClient;

        private PlayerData _playerOneData;
        private PlayerData _playerTwoData;
        private PlayerData _currentPlayerData;
        private PlayerData _otherPlayerData => _currentPlayerData == _playerOneData ? _playerTwoData : _playerOneData;

        private bool _gameRunning;
        private Random _random;


        private StateMachine _stateMachine;

        public HanamikojiGame()
        {
            _gameRunning = true;
            _playerOneData = new PlayerData();
            _playerTwoData = new PlayerData();
            _random = new Random();
            _cardDeck = new List<GiftCard>();

            
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
            _stateMachine = new StateMachine(new BeginRoundState(this));

            while (_gameRunning)
            {
                CheckIfPlayerDisconnected();

                _stateMachine.Execute();

                Thread.Sleep(10);
            }

            DisconnectPlayer(_playerOneTcpClient);
            DisconnectPlayer(_playerTwoTcpClient);
        }

        public async Task<Packet?> ReadFromCurrentPlayer() => await ReadFromPlayer(_currentPlayer);
        public async Task<Packet?> ReadFromOtherPlayer() => await ReadFromPlayer(_otherPlayer);
        private async Task<Packet?> ReadFromPlayer(TcpClient player)
        {
            try
            {
                if (player.Available > 0)
                {
                    return await PacketProcessing.ReceivePacket(player.GetStream());
                }
            }
            catch (Exception exception)
            {
                ConsoleWrapper.WriteError(exception.Message);
            }

            return null;
        }

        public void SendToCurrentPlayer(PacketCommandEnum command, string message = "")
        =>
            SendToPlayer(_currentPlayer, command, message);

        public void SendToOtherPlayer(PacketCommandEnum command, string message = "")
        =>
            SendToPlayer(_otherPlayer, command, message);

        public void SendToPlayer(TcpClient player, PacketCommandEnum command, string message)
        {
            if (player == null) return;

            PacketProcessing.SendPacket(player.GetStream(), new Packet(command, message))
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
            SendGameDataToCurrentPlayer();
            SendGameDataToOtherPlayer();
        }

        public void SendGameDataToCurrentPlayer() =>
            SendToCurrentPlayer(PacketCommandEnum.GameState, (new GameData(_currentPlayerData, _otherPlayerData.AnonimizeData())).SerializeToJson());

        public void SendGameDataToOtherPlayer() =>
            SendToOtherPlayer(PacketCommandEnum.GameState, (new GameData(_otherPlayerData, _currentPlayerData.AnonimizeData())).SerializeToJson());

        public void SendCompromiseOfferToOtherPlayer(List<GiftCard> compromiseCardsToOffer)
        {
            var gameStateToSend = new GameData(_otherPlayerData, _currentPlayerData.AnonimizeData(),
                new List<PlayerMoveTypeEnum> { PlayerMoveTypeEnum.CompromiseOffer });

            gameStateToSend.SetCompromiseCards(compromiseCardsToOffer);

            SendToOtherPlayer(PacketCommandEnum.GameState, gameStateToSend.SerializeToJson());
        }
        public void SendDoubleGiftOfferToOtherPlayer(List<GiftCard> compromiseCardsToOffer)
        {
            var gameStateToSend = new GameData(_otherPlayerData, _currentPlayerData.AnonimizeData(),
                new List<PlayerMoveTypeEnum> { PlayerMoveTypeEnum.DoubleGiftOffer });

            gameStateToSend.SetDoubleGiftCards(compromiseCardsToOffer);

            SendToOtherPlayer(PacketCommandEnum.GameState, gameStateToSend.SerializeToJson());
        }

        public void StartNewRound()
        {
            _cardDeck = new List<GiftCard>(GiftCardConstants.GetAllCards());
            _currentPlayerData.ClearData();
            _otherPlayerData.ClearData();
            DrawRandomCardsToCurrentPlayer(6);
            DrawRandomCardsToOtherPlayer(6);
        }

        public void StopGame(string reason = "")
        {
            _gameRunning = false;
            Console.WriteLine($"[GAME {GameIdentifier}] : Stopping the Game because: {reason}");
        }

        public PlayerData GetCurrentPlayerData() => _currentPlayerData;

        public PlayerData GetOtherPlayerData() => _otherPlayerData;

        public void DrawRandomCardsToCurrentPlayer(int numberOfCards)
        {
            DrawRandomCardsToPlayer(_currentPlayerData, numberOfCards);
        }

        public void DrawRandomCardsToOtherPlayer(int numberOfCards)
        {
            DrawRandomCardsToPlayer(_otherPlayerData, numberOfCards);
        }

        public void DrawRandomCardsToPlayer(PlayerData playerData, int numberOfCards) =>
            playerData.CardsOnHand.AddRange(GetRandomCards(numberOfCards));

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
                PacketProcessing.SendPacket(_playerTwoTcpClient.GetStream(), new Packet(PacketCommandEnum.Error, "other player disconnected"))
                    .GetAwaiter().GetResult();
                StopGame("Player one left the game");
            }

            if (CheckIfPlayerDisconnected(_playerTwoTcpClient))
            {
                PacketProcessing.SendPacket(_playerOneTcpClient.GetStream(), new Packet(PacketCommandEnum.Error, "other player disconnected"))
                    .GetAwaiter().GetResult();
                StopGame("Player two left the game");
            }
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

        private List<GiftCard> GetRandomCards(int numberOfCards)
        {
            var cardsForPlayer = new List<GiftCard>();
            for (int i = 0; i < numberOfCards; i++)
            {
                var randomCardIndex = _random.Next(_cardDeck.Count());
                cardsForPlayer.Add(_cardDeck[randomCardIndex]);
                _cardDeck.RemoveAt(randomCardIndex);
            }

            return cardsForPlayer;
        }
    }
}
