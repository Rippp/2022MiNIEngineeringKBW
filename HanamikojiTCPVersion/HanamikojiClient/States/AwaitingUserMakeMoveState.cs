using CommonResources;
using CommonResources.Game;
using CommonResources.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiClient.States
{
    internal class AwaitingUserMakeMoveState : AbstractClientState
    {
        private PlayerData _playerData;
        private MoveData _moveData;
        public AwaitingUserMakeMoveState(TcpGameClient client) : base(client) { }

        public override void EnterState()
        {
            Console.WriteLine("Entered State: AwaitingUserMakeMoveState");
            _playerData = _client.GetPlayerData();
            _moveData = new MoveData();
        }
        
        public override AbstractClientState? DoWork()
        {
            var selectedMoveType = ConsoleWrapper.PromptSingleSelection(_playerData.GetAvailableMoves(), customTitle: "Select move:");
            _moveData.MoveType = selectedMoveType;

            var selectedCards = ConsoleWrapper.PromptMultipleCardsSelection(_playerData.CardsOnHand, GetNumberOfCardsToChoose(selectedMoveType), 
                selectedMoveType.ToString());
            _moveData.GiftCards = new List<GiftCard>(selectedCards);

            _client.SendToServer(PacketCommandEnum.PlayerMove, _moveData.SerializeToJson());

            return new AwaitingServerMoveValidationState(_client);
        }

        public override void ExitState()
        {

        }

        private int GetNumberOfCardsToChoose(PlayerMoveTypeEnum playerMoveType)
        {
            switch(playerMoveType)
            {
                case PlayerMoveTypeEnum.Elimination:
                    return 2;
                case PlayerMoveTypeEnum.DoubleGift:
                    return 4;
                case PlayerMoveTypeEnum.Secret:
                    return 1;
                case PlayerMoveTypeEnum.Compromise:
                    return 3;
            }
            return 0;   
        }
    }
}
