using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers.Moves;

public class SecretMoveHandler : MoveHandler
{
    private readonly IDictionary<GiftCardEntity, DateTime> _nextPossibleCardSelectionDictionary = new Dictionary<GiftCardEntity, DateTime>();
    private readonly HashSet<GiftCardEntity> _selectedCards = new HashSet<GiftCardEntity>();

    public SecretMoveHandler(IDictionary<Guid, GiftCardEntity> giftCardEntityDictionary) : base(giftCardEntityDictionary)
    {
    }

    public override void Update(GameData gameData, MouseState mouseState)
    {
        bool isAnyCardPointed = false;

        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        foreach (var card in gameData.CurrentPlayerData.CardsOnHand)
        {
            var cardEntity = _giftCardEntityDictionary[card.CardId];

            if (cardEntity.IsPointInsideLeftHalfOfSprite(mousePosition))
            {
                isAnyCardPointed = true;

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
                else
                {
                   // ChangeEnlargedEntity(cardEntity);
                }
            }
        }

        if (!isAnyCardPointed)
        {
           // ClearEnlargedEntity();
        }
    }

    public override bool Validate()
    {
        return true;
    }

    public override MoveData GetMoveData(GameData gameData)
    {
        var selectedCardIds = _selectedCards.Select(x => x.CardId).ToList();

        return new MoveData
        {
            MoveType = PlayerMoveTypeEnum.Secret,
            GiftCards = gameData.GetAllCards().Where(x => selectedCardIds.Contains(x.CardId)).ToList()
        };
    }

    public override void Clear()
    {
        _selectedCards.Clear();
    }

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