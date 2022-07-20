using CommonResources.Game;

namespace HanamikojiServer.States
{
    public class ExecuteCurrentPlayerMoveState : AbstractServerState
    {
        private readonly MoveData _moveData;
        private readonly PlayerData _currentPlayerData;
        private bool _moveExecuted = false;

        public ExecuteCurrentPlayerMoveState(HanamikojiGame game, MoveData moveData) : base(game) 
        {
            _moveData = moveData;
            _currentPlayerData = _game.GetCurrentPlayerData();
        }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: ExecuteMoveState");

            switch(_moveData.MoveType)
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

                default:
                    break;
            }

            _currentPlayerData.MarkMoveAsNotAvailable(_moveData.MoveType);
        }

        public override AbstractServerState? DoWork()
        {
            if (_moveExecuted) 
                return new CurrentPlayerEndTurnState(_game);

            return new AwaitOtherPlayerMoveState(_game, _moveData);
        }

        public override void ExitState()
        {

        }

        private void ExecuteCompromiseMove()
        {
            foreach (var card in _moveData.GiftCards) RemoveCardFromCurrentPlayerHand(card);
            _game.SendCompromiseOfferToOtherPlayer(_moveData.GiftCards);
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

        private void RemoveCardFromCurrentPlayerHand(GiftCard cardToRemove) 
            => _currentPlayerData.CardsOnHand
            .RemoveAt(_currentPlayerData.CardsOnHand.FindIndex(x => x.Type == cardToRemove.Type));
    }
}
