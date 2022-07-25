using CommonResources.Game;
using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiServer.States
{
    public class ValidateOtherPlayerMoveState : AbstractServerState
    {
        private readonly MoveData _currentPlayerMoveData;
        private readonly MoveData _otherPlayerMoveData;
        public ValidateOtherPlayerMoveState(HanamikojiGame game, MoveData currentPlayerMoveData,
            MoveData otherPlayerMoveData) : base(game)
        {
            _currentPlayerMoveData = currentPlayerMoveData;
            _otherPlayerMoveData = otherPlayerMoveData;
        }
        public override void EnterState()
        {
            Console.WriteLine("Entered State: ValidateOtherPlayerMoveState");
        }

        public override AbstractServerState DoWork()
        {
            _game.SendToOtherPlayer(PacketCommandEnum.MoveVerified);

            return new ExecuteOtherPlayerMoveState(_game, new MoveData
            {
                MoveType = _otherPlayerMoveData.MoveType,
                GiftCards = _currentPlayerMoveData.GiftCards,
                AnswerGiftCards = _otherPlayerMoveData.GiftCards
            });
        }

        public override void ExitState()
        {

        }

        private bool CheckIfMoveContainsRightAmountOfCards(PlayerMoveTypeEnum moveType, int cardsReceived)
        {
            switch (moveType)
            {
                case PlayerMoveTypeEnum.CompromiseResponse:
                    return cardsReceived == 1;

                case PlayerMoveTypeEnum.DoubleGiftResponse:
                    return cardsReceived == 2;
            }

            return false;
        }
    }
}
