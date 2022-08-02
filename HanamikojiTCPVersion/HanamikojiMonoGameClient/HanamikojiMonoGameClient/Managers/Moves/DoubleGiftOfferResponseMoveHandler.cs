using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanamikojiMonoGameClient.Managers.Moves
{
    public class DoubleGiftOfferResponseMoveHandler : MoveHandler
    {
        private readonly ClickedEntityProvider _clickedEntityProvider;

        private List<GiftCardEntity> _selectedDoubleGiftCardsValue = null;
        private List<GiftCardEntity> _selectedDoubleGiftCards
        {
            get => _selectedDoubleGiftCardsValue;
            set
            {
                if (_selectedDoubleGiftCardsValue != null)
                    foreach (var cardEntity in _selectedDoubleGiftCardsValue)
                        cardEntity.MoveInY(-50);

                if (value != null)
                    foreach (var cardEntity in value)
                        cardEntity.MoveInY(50);

                _selectedDoubleGiftCardsValue = value;
            }
        }

        public DoubleGiftOfferResponseMoveHandler(IEntitiesRepository entitiesRepository, ClickedEntityProvider clickedEntityProvider) : base(entitiesRepository)
        {
            _clickedEntityProvider = clickedEntityProvider;
        }

        public override void Update(GameData gameData, MouseState mouseState)
        {
            var clickedDoubleGiftCards = _clickedEntityProvider.GetClickedDoubleGiftOfferCardEntities();

            if (clickedDoubleGiftCards != null) _selectedDoubleGiftCards = clickedDoubleGiftCards;
        }

        public override MoveData GetMoveData(GameData gameData)
        {
            var selectedCardsIds = _selectedDoubleGiftCards.Select(x => x.CardId).ToList();

            return new MoveData
            {
                MoveType = PlayerMoveTypeEnum.DoubleGiftResponse,
                GiftCards = gameData.DoubleGiftCards.Where(x => selectedCardsIds.Contains(x.CardId)).ToList()
            };
        }

        public override void Clear()
        {
            _selectedDoubleGiftCards?.Clear();
        }
    }
}
