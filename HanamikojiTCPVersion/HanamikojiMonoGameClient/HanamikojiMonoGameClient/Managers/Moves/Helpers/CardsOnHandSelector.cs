using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Providers;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers.Moves.Helpers;

public class CardsOnHandSelector
{
    private readonly IDictionary<GiftCardEntity, DateTime> _nextPossibleCardSelectionDictionary = new Dictionary<GiftCardEntity, DateTime>();
    private readonly HashSet<GiftCardEntity> _selectedCards = new HashSet<GiftCardEntity>();
    private IPointedEntityProvider _pointedEntityProvider;

    public CardsOnHandSelector(IPointedEntityProvider pointedEntityProvider)
    {
        _pointedEntityProvider = pointedEntityProvider;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            var clickedCardEntity = _pointedEntityProvider.GetPointedCardOnHand(gameData, mouseState);
            if (clickedCardEntity != null)
            {
                if (_selectedCards.Contains(clickedCardEntity))
                {
                    DeselectCard(clickedCardEntity);
                }
                else
                {
                    SelectCard(clickedCardEntity);
                }
            }
        }
    }

    public List<GiftCardEntity> GetSelectedCardEntities() => _selectedCards.ToList();

    public void Clear() => _selectedCards.Clear();

    private void SelectCard(GiftCardEntity cardEntity)
    {
        if (_nextPossibleCardSelectionDictionary.ContainsKey(cardEntity) && DateTime.Now < _nextPossibleCardSelectionDictionary[cardEntity]) return;
        cardEntity.MoveInY(-50);
        _selectedCards.Add(cardEntity);
        _nextPossibleCardSelectionDictionary[cardEntity] = DateTime.Now.AddMilliseconds(GameSettings.MILISECONDS_BETWEEN_ACTIONS);
    }

    private void DeselectCard(GiftCardEntity cardEntity)
    {
        if (_nextPossibleCardSelectionDictionary.ContainsKey(cardEntity) && DateTime.Now < _nextPossibleCardSelectionDictionary[cardEntity]) return;
        cardEntity.MoveInY(50);
        _selectedCards.Remove(cardEntity);
        _nextPossibleCardSelectionDictionary[cardEntity] = DateTime.Now.AddMilliseconds(GameSettings.MILISECONDS_BETWEEN_ACTIONS);
    }
}