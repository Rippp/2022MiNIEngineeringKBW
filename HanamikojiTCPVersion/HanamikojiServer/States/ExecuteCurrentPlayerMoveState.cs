using CommonResources.Game;

namespace HanamikojiServer.States
{
    public class ExecuteCurrentPlayerMoveState : AbstractServerState
    {
        private readonly MoveData _moveData;
        private readonly PlayerData _currentPlayerData;
        private readonly PlayerData _otherPlayerData;
        private bool _moveExecuted = false;

        public ExecuteCurrentPlayerMoveState(HanamikojiGame game, MoveData moveData) : base(game)
        {
            _moveData = moveData;
            _currentPlayerData = _game.GetCurrentPlayerData();
            _otherPlayerData = _game.GetOtherPlayerData();
        }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: ExecuteMoveState");

            switch (_moveData.MoveType)
            {
                case PlayerMoveTypeEnum.Secret:
                    ExecuteSecretMove();
                    break;

                case PlayerMoveTypeEnum.Elimination:
                    ExecuteEliminationMove();
                    break;

                case PlayerMoveTypeEnum.Compromise:
                    ExecuteCompromiseMove();
                    break;

                case PlayerMoveTypeEnum.DoubleGift:
                    ExecuteDoubleGiftMove();
                    break;

                case PlayerMoveTypeEnum.CompromiseResponse:
                case PlayerMoveTypeEnum.DoubleGiftResponse:
                    ExecuteTradeResponseMove();
                    break;

                default:
                    break;
            }

            _currentPlayerData.MarkMoveAsNotAvailable(_moveData.MoveType);
        }

        public override AbstractServerState? DoWork()
        {
            if (_moveExecuted)
                return new CurrentPlayerEndTurnState(_game);

            return new AwaitCurrentPlayerMoveState(_game, _moveData);
        }

        public override void ExitState() { }

        private void ExecuteCompromiseMove()
        {
            foreach (var card in _moveData.GiftCards) RemoveCardFromCurrentPlayerHand(card);
            _game.SendCompromiseOfferToOtherPlayer(_moveData.GiftCards);
            _game.SwitchPlayer();
        }

        private void ExecuteDoubleGiftMove()
        {
            foreach (var card in _moveData.GiftCards) RemoveCardFromCurrentPlayerHand(card);
            _game.SendDoubleGiftOfferToOtherPlayer(_moveData.GiftCards);
            _game.SwitchPlayer();
        }

        private void ExecuteSecretMove()
        {
            _currentPlayerData.SecretCard = _moveData.GiftCards.First();
            RemoveCardFromCurrentPlayerHand(_moveData.GiftCards.First());
            _moveExecuted = true;
        }

        private void ExecuteEliminationMove()
        {
            _currentPlayerData.EliminationCards = _moveData.GiftCards;
            foreach (var card in _moveData.GiftCards) RemoveCardFromCurrentPlayerHand(card);
            _moveExecuted = true;
        }

        private void ExecuteTradeResponseMove()
        {
            _currentPlayerData.GiftsFromPlayer.AddRange(_moveData.GiftCards);
            _otherPlayerData.GiftsFromPlayer.AddRange(_moveData.TradeMoveGiftCards
                .Where(x => !_moveData.GiftCards.Any(y => x.CardId == y.CardId)));

            _game.SwitchPlayer();
            _moveExecuted = true;
        }

        private void RemoveCardFromCurrentPlayerHand(GiftCard cardToRemove)
            => _currentPlayerData.CardsOnHand.RemoveAll(x => x.CardId == cardToRemove.CardId);
    }
}
