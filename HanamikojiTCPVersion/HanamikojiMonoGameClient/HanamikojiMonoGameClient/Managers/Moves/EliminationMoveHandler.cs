using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.Managers.Moves.Helpers;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers.Moves;

public class EliminationMoveHandler : MoveHandler
{
    private readonly CardsOnHandSelector _cardsOnHandSelector;
    private const int CardsToChoose = 2;

    public EliminationMoveHandler(IEntitiesRepository entitiesRepository, CardsOnHandSelector cardsOnHandSelector) : base(entitiesRepository)
    {
        _cardsOnHandSelector = cardsOnHandSelector;
    }

    public override void Update(GameData gameData, MouseState mouseState)
    {
        _cardsOnHandSelector.Update(gameData, mouseState);
    }

    public override bool Validate()
    {
        var selectedCards = _cardsOnHandSelector.GetSelectedCardEntities();
        // print validation info?
        return selectedCards.Count == CardsToChoose;
    }

    public override MoveData GetMoveData(GameData gameData)
    {
        var selectedCardIds = _cardsOnHandSelector.GetSelectedCardEntities().Select(x => x.CardId).ToList();

        return new MoveData
        {
            MoveType = PlayerMoveTypeEnum.Elimination,
            GiftCards = gameData.GetAllCards().Where(x => selectedCardIds.Contains(x.CardId)).ToList()
        };
    }

    public override void Clear()
    {
        _cardsOnHandSelector.Clear();
    }
}