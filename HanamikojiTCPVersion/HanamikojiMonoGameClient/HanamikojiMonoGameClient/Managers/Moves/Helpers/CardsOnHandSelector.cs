using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers.Moves.Helpers;

public class CardsOnHandSelector
{
    private readonly IDictionary<GiftCardEntity, DateTime> _nextPossibleCardSelectionDictionary = new Dictionary<GiftCardEntity, DateTime>();
    private readonly HashSet<GiftCardEntity> _selectedCards = new HashSet<GiftCardEntity>();
    private ClickedEntityProvider _clickedEntityProvider;

    public CardsOnHandSelector(ClickedEntityProvider clickedEntityProvider)
    {
        _clickedEntityProvider = clickedEntityProvider;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        var clickedCardEntity = _clickedEntityProvider.GetClickedCardOnHandEntity();

        if(clickedCardEntity != null)
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