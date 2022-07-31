using CommonResources;
using CommonResources.Game;
using CommonResources.Network;

namespace HanamikojiServer.States
{
    public class ValidateCurrentPlayerMoveState : AbstractServerState
    {
        private readonly MoveData _moveToValidate;
        private readonly PlayerData _currentPlayerData;
        public ValidateCurrentPlayerMoveState(HanamikojiGame game, MoveData moveToValidate) : base(game)
        {
            _moveToValidate = moveToValidate;
            _currentPlayerData = _game.GetCurrentPlayerData();
        }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: ValidateCurrentPlayerMoveState");
        }

        public override AbstractServerState DoWork()
        {
            if (!CheckIfReceivedCardsAvailableInCollection(_moveToValidate.GiftCards, _currentPlayerData.CardsOnHand) &&
                !CheckIfReceivedCardsAvailableInCollection(_moveToValidate.GiftCards, _moveToValidate.TradeMoveGiftCards))
            {
                InvalidateMove("Move contains cards that were not available for selection");
                return new AwaitCurrentPlayerMoveState(_game);
            }

            if (!CheckIfMoveContainsRightAmountOfCards(_moveToValidate.MoveType, _moveToValidate.GiftCards.Count()))
            {
                InvalidateMove("Move doesn't contain right amount of cards");
                return new AwaitCurrentPlayerMoveState(_game);
            }

            return new ExecuteCurrentPlayerMoveState(_game, _moveToValidate);
        }

        public override void ExitState()
        {

        }

        private bool CheckIfMoveContainsRightAmountOfCards(PlayerMoveTypeEnum moveType, int cardsReceived)
        {
            switch (moveType)
            {
                case PlayerMoveTypeEnum.Secret:
                    return cardsReceived == 1;

                case PlayerMoveTypeEnum.Elimination:
                    return cardsReceived == 2;

                case PlayerMoveTypeEnum.Compromise:
                    return cardsReceived == 3;

                case PlayerMoveTypeEnum.DoubleGift:
                    return cardsReceived == 4;

                case PlayerMoveTypeEnum.DoubleGiftResponse:
                    return cardsReceived == 2;

                case PlayerMoveTypeEnum.CompromiseResponse:
                    return cardsReceived == 1;
            }

            return false;
        }

        private bool CheckIfReceivedCardsAvailableInCollection(List<GiftCard> receivedCards, List<GiftCard>? giftCardCollection)
        {
            if (giftCardCollection == null) return false;

            var moveGiftCardsGroupped = receivedCards.GroupBy(x => x.Type).Select(x => new { GeishaType = x.Key, Count = x.Count() }).ToList();

            foreach (var geishaType in moveGiftCardsGroupped)
                if (geishaType.Count > giftCardCollection.Where(x => x.Type == x.Type).Count())
                    return false;

            return true;
        }
        private void InvalidateMove(string error)
        {
            _game.SendGameDataToCurrentPlayer(error);
        }
    }
}
