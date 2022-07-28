using System;
using System.Collections.Generic;
using CommonResources;
using CommonResources.Game;
using CommonResources.Network;
using HanamikojiClient.States;

namespace HanamikojiMonoGameClient.States
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
    }
}
