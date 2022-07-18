using CommonResources.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    public class ExecuteMoveState : AbstractServerState
    {
        private readonly MoveData _moveData;
        private readonly PlayerData _currentPlayerData;
        private readonly PlayerData _otherPlayerData;
        public ExecuteMoveState(HanamikojiGame game, MoveData moveData) : base(game) 
        {
            _moveData = moveData;
            _currentPlayerData = _game.GetCurrentPlayerData();
            _otherPlayerData = _game.GetOtherPlayerData();
        }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: ExecuteMoveState");

            switch(_moveData.MoveType)
            {
                case PlayerMoveTypeEnum.Secret:
                    _currentPlayerData.SecretCard = _moveData.GiftCards.First();
                    RemoveCardFromCurrentPlayerHand(_moveData.GiftCards.First());
                    break;

                case PlayerMoveTypeEnum.Elimination:
                    _currentPlayerData.EliminationCards = _moveData.GiftCards;
                    foreach (var card in _moveData.GiftCards) RemoveCardFromCurrentPlayerHand(card);
                    break;

                default:
                    break;
            }
        }

        public override AbstractServerState? DoWork()
        {
            return new CurrentPlayerEndTurnState(_game);
        }

        public override void ExitState()
        {

        }

        private void RemoveCardFromCurrentPlayerHand(GiftCard cardToRemove) 
            => _currentPlayerData.CardsOnHand
            .RemoveAt(_currentPlayerData.CardsOnHand.FindIndex(x => x.Type == cardToRemove.Type));
    }
}
