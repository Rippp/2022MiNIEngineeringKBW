using CommonResources.Game;
using HanamikojiMonoGameClient.Animations;
using HanamikojiMonoGameClient.Managers.Moves.Helpers;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace HanamikojiMonoGameClient.Managers.Moves
{
    public class DoubleGiftMoveHandler : MoveHandler
    {
        private readonly CardsOnHandSelector _cardsOnHandSelector;
        private const int CardsToChoose = 4;

        public DoubleGiftMoveHandler(IEntitiesRepository entitiesRepository, CardsOnHandSelector cardsOnHandSelector) : base(entitiesRepository)
        {
            _cardsOnHandSelector = cardsOnHandSelector;
        }

        public override void Update(GameData gameData, MouseState mouseState)
        {
            _cardsOnHandSelector.Update(gameData, mouseState);
        }

        public override MoveData GetMoveData(GameData gameData)
        {
            var selectedCardsIds = _cardsOnHandSelector.GetSelectedCardEntities().Select(x => x.CardId).ToList();

            return new MoveData
            {
                MoveType = PlayerMoveTypeEnum.DoubleGift,
                GiftCards = gameData.GetAllCards().Where(x => selectedCardsIds.Contains(x.CardId)).ToList()
            };
        }

        public override bool Validate() => _cardsOnHandSelector.GetSelectedCardEntities().Count == CardsToChoose;

        public override void Clear()
        {
            _cardsOnHandSelector.Clear();
        }
    }
}
