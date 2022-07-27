using CommonResources;
using CommonResources.Game;
using CommonResources.Network;
using Spectre.Console;

namespace HanamikojiClient.States
{
    internal class AwaitingUserMakeMoveState : AbstractClientState
    {
        private GameData _gameData;
        private MoveData _moveData;
        public AwaitingUserMakeMoveState(TcpGameClient client) : base(client) { }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: AwaitingUserMakeMoveState");
            _gameData = _client.GetGameData();
            _moveData = new MoveData();
        }

        public override AbstractClientState? DoWork()
        {
            PrintGameState();

            if (_gameData.MovesAvailable.Contains(PlayerMoveTypeEnum.CompromiseOffer))
                HandleCompromiseOffer();
            else if (_gameData.MovesAvailable.Contains(PlayerMoveTypeEnum.DoubleGiftOffer))
                HandleDoubleGiftOffer();
            else
                HandleRegularMove();

            return new AwaitingPacketState(_client);
        }

        public override void ExitState()
        {
            _client.SendToServer(PacketCommandEnum.PlayerMove, _moveData.SerializeToJson());
        }

        private void HandleCompromiseOffer()
        {
            _moveData.MoveType = PlayerMoveTypeEnum.CompromiseResponse;

            var selectedCard = ConsoleWrapper.PromptSingleSelection(_gameData.CompromiseCards,
                customTitle: "Choose compromise card: ", optionStyleFunction: ConsoleWrapper.GiftCardStyleFunc);

            _moveData.GiftCards = new List<GiftCard>() { selectedCard };
        }

        private void HandleDoubleGiftOffer()
        {
            _moveData.MoveType = PlayerMoveTypeEnum.DoubleGiftResponse;

            var pairs = new List<(GiftCard card1, GiftCard card2)>() {
                (_gameData.DoubleGiftCards[0], _gameData.DoubleGiftCards[1]),
                (_gameData.DoubleGiftCards[2], _gameData.DoubleGiftCards[3]),
            };

            var selectedPair = ConsoleWrapper.PromptSingleSelection(pairs,
                customTitle: "Choose double gift card pair: ", optionStyleFunction: ConsoleWrapper.GiftCardPairStyleFunc);

            _moveData.GiftCards = new List<GiftCard>() { selectedPair.card1, selectedPair.card2 };
        }

        private void HandleRegularMove()
        {
            var selectedMoveType = ConsoleWrapper.PromptSingleSelection(_gameData.MovesAvailable, customTitle: "Select move:");
            _moveData.MoveType = selectedMoveType;

            var selectedCards = ConsoleWrapper.PromptMultipleCardsSelection(_gameData.CurrentPlayerData.CardsOnHand, GetNumberOfCardsToChoose(selectedMoveType),
                selectedMoveType.ToString());
            _moveData.GiftCards = new List<GiftCard>(selectedCards);
        }

        private int GetNumberOfCardsToChoose(PlayerMoveTypeEnum playerMoveType)
        {
            switch (playerMoveType)
            {
                case PlayerMoveTypeEnum.Secret:
                    return 1;
                case PlayerMoveTypeEnum.Elimination:
                    return 2;
                case PlayerMoveTypeEnum.Compromise:
                    return 3;
                case PlayerMoveTypeEnum.DoubleGift:
                    return 4;
            }
            return 0;
        }

        private void PrintGameState()
        {
            ConsoleWrapper.PrintGeishaStates(_gameData.CurrentPlayerData, _gameData.OtherPlayerData,
                "CurrentPlayer", "OtherPlayer ");
            const string HandLabel = "Hand";
            const string SecretLabel = "Secret";
            const string EliminationCardsLabel = "Elimination Cards";

            // hand
            var cardsInHandPlayerOneTable = ConsoleWrapper.GetCardsTable(_gameData.CurrentPlayerData.CardsOnHand, HandLabel);
            var cardsInHandPlayerTwoTable = ConsoleWrapper.GetCardsTable(_gameData.OtherPlayerData.CardsOnHand, HandLabel);

            // secret
            var secretsPlayerOne = _gameData.CurrentPlayerData.SecretCard is not null ? new List<GiftCard> { _gameData.CurrentPlayerData.SecretCard } : new List<GiftCard>();
            var secretsPlayerTwo = _gameData.OtherPlayerData.SecretCard is not null ? new List<GiftCard> { _gameData.OtherPlayerData.SecretCard } : new List<GiftCard>();
            var playerOneSecretTable = ConsoleWrapper.GetCardsTable(secretsPlayerOne, SecretLabel);
            var playerTwoSecretTable = ConsoleWrapper.GetCardsTable(secretsPlayerTwo, SecretLabel);

            // elimination
            var eliminationPlayerOne = _gameData.CurrentPlayerData.EliminationCards is not null
                ? new List<GiftCard>(_gameData.CurrentPlayerData.EliminationCards)
                : new List<GiftCard>();
            var eliminationPlayerTwo = _gameData.OtherPlayerData.EliminationCards is not null
                ? new List<GiftCard>(_gameData.OtherPlayerData.EliminationCards)
                : new List<GiftCard>();
            var eliminationPlayerOneTable = ConsoleWrapper.GetCardsTable(eliminationPlayerOne, EliminationCardsLabel);
            var eliminationPlayerTwoTable = ConsoleWrapper.GetCardsTable(eliminationPlayerTwo, EliminationCardsLabel);

            // First column
            var tablePlayerOne = new Table();
            tablePlayerOne.AddColumn(new TableColumn("Current Player"));
            tablePlayerOne.AddRow(cardsInHandPlayerOneTable);
            tablePlayerOne.AddRow(playerOneSecretTable);
            tablePlayerOne.AddRow(eliminationPlayerOneTable);

            // Second column
            var tablePlayerTwo = new Table();
            tablePlayerTwo.AddColumn(new TableColumn("Other Player"));
            tablePlayerTwo.AddRow(cardsInHandPlayerTwoTable);
            tablePlayerTwo.AddRow(playerTwoSecretTable);
            tablePlayerTwo.AddRow(eliminationPlayerTwoTable);

            var table = new Table();
            table.AddColumn(new TableColumn(tablePlayerOne));
            table.AddColumn(new TableColumn(tablePlayerTwo));
            AnsiConsole.Write(table);
        }
    }
}
