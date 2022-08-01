using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers;

public class CardsOnHandSelector
{
    private readonly IDictionary<GiftCardEntity, DateTime> _nextPossibleCardSelectionDictionary = new Dictionary<GiftCardEntity, DateTime>();
    private readonly HashSet<GiftCardEntity> _selectedCards = new HashSet<GiftCardEntity>();
    private readonly IDictionary<Guid, GiftCardEntity> _giftCardEntityDictionary;

    public CardsOnHandSelector(IDictionary<Guid, GiftCardEntity> giftCardEntityDictionary)
    {
        _giftCardEntityDictionary = giftCardEntityDictionary;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        foreach (var card in gameData.CurrentPlayerData.CardsOnHand)
        {
            var cardEntity = _giftCardEntityDictionary[card.CardId];

            if (cardEntity.IsPointInsideLeftHalfOfSprite(mousePosition))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (_selectedCards.Contains(cardEntity))
                    {
                        DeselectCard(cardEntity);
                    }
                    else
                    {
                        SelectCard(cardEntity);
                    }
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