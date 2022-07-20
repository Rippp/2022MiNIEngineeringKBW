using CommonResources.Game;
using CommonResources.Network;
using System.Linq;

namespace HanamikojiServer.States
{
    public class ValidateCurrentPlayerMoveState : AbstractServerState
    {
        private readonly MoveData _moveData;
        private PlayerData _currentPlayerData;
        public ValidateCurrentPlayerMoveState(HanamikojiGame game, MoveData moveData) : base(game)
        {
            _moveData = moveData;
        }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: ValidateCurrentPlayerMoveState");
            _currentPlayerData = _game.GetCurrentPlayerData();
        }

        public override AbstractServerState DoWork()
        {
            if (!CheckIfMoveAvailable(_moveData.MoveType))
            {
                InvalidateMove("Move is unavailable");
                return new AwaitCurrentPlayerMoveState(_game);
            }

            if (!CheckIfReceivedCardsAreAvailableInHand(_moveData.GiftCards))
            {
                InvalidateMove("Move contains cards that are not in current player's hand");
                return new AwaitCurrentPlayerMoveState(_game);
            }

            if (!CheckIfMoveContainsRightAmountOfCards(_moveData.MoveType, _moveData.GiftCards.Count()))
            {
                InvalidateMove("Move doesn't contain right amount of cards");
                return new AwaitCurrentPlayerMoveState(_game);
            }

            ValidateMove();
            return new ExecuteCurrentPlayerMoveState(_game, _moveData);    // TEMPORARY, HERE WE SHOULD BEGIN MOVE PROCESSING
        }

        public override void ExitState()
        {

        }

        private bool CheckIfMoveContainsRightAmountOfCards(PlayerMoveTypeEnum moveType, int cardsReceived)
        {
            switch (moveType)
            {
                case PlayerMoveTypeEnum.Compromise:
                    return cardsReceived == 3;

                case PlayerMoveTypeEnum.DoubleGift:
                    return cardsReceived == 4;

                case PlayerMoveTypeEnum.Elimination:
                    return cardsReceived == 2;

                case PlayerMoveTypeEnum.Secret:
                    return cardsReceived == 1;
            }

            return false;
        }

        private bool CheckIfMoveAvailable(PlayerMoveTypeEnum playerMove) =>
            _currentPlayerData.movesAvailability[playerMove];

        private bool CheckIfReceivedCardsAreAvailableInHand(List<GiftCard> moveCards)
        {
            var moveGiftCardsGroupped = moveCards.GroupBy(x => x.Type).Select(x => new { GeishaType = x.Key, Count = x.Count() }).ToList();

            foreach (var geishaType in moveGiftCardsGroupped)
                if (geishaType.Count > _currentPlayerData.CardsOnHand.Where(x => x.Type == x.Type).Count())
                    return false;

            return true;
        }

        private void InvalidateMove(string error)
        {
            _game.SendToCurrentPlayer(PacketCommandEnum.MoveInvalid, error);
            _game.SendGameDataToCurrentPlayer();
        }

        private void ValidateMove()
        {
            _game.SendToCurrentPlayer(PacketCommandEnum.MoveVerified);
        }
    }
}
