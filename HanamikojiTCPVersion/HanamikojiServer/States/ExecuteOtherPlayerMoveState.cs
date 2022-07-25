using CommonResources.Game;
using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    public class ExecuteOtherPlayerMoveState : AbstractServerState
    {
        private readonly MoveData _moveData;
        private PlayerData _currentPlayerData;
        private PlayerData _otherPlayerData;
        public ExecuteOtherPlayerMoveState(HanamikojiGame game, MoveData moveData) : base(game)
        {
            _moveData = moveData;
        }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: ExecuteOtherPlayerMoveState");

            _currentPlayerData = _game.GetCurrentPlayerData();
            _otherPlayerData = _game.GetOtherPlayerData();
        }

        public override AbstractServerState DoWork()
        {
            switch (_moveData.MoveType)
            {
                case PlayerMoveTypeEnum.CompromiseResponse:
                case PlayerMoveTypeEnum.DoubleGiftResponse:
                    _otherPlayerData.GiftsFromPlayer.AddRange(_moveData.AnswerGiftCards);
                    _currentPlayerData.GiftsFromPlayer.AddRange(_moveData.GiftCards
                        .Where(x => !_moveData.AnswerGiftCards.Any(y => x.CardId == y.CardId)));
                    break;
                default:
                    return null;
            }

            return new CurrentPlayerEndTurnState(_game);
        }

        public override void ExitState()
        {

        }
    }
}
