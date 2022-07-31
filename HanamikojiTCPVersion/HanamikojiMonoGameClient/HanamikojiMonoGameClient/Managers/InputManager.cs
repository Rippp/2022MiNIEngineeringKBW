using System;
using System.Collections.Generic;
using System.Net.Sockets;
using CommonResources.Game;
using HanamikojiClient;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace HanamikojiMonoGameClient.Managers;

public class InputManager
{
    private readonly List<GameEntity> _gameEntities;
    private readonly TcpGameClient _tcpGameClient;
    
    private readonly IDictionary<Guid, GiftCardEntity> _giftCardEntityDictionary;
    private readonly Button _submitButton;
    
    private MoveCardEntity _selectedMove;
    private GameData _lastReceivedGameData = null;

    private readonly IDictionary<GiftCardEntity, DateTime> _nextPossibleCardSelectionDictionary = new Dictionary<GiftCardEntity, DateTime>();
    private GameEntity _enlargedEntity; 
    private readonly HashSet<GiftCardEntity> _selectedCards = new HashSet<GiftCardEntity>();

    public InputManager(List<GameEntity> gameEntities, IDictionary<Guid, GiftCardEntity>  giftCardEntityDictionary, TcpGameClient tcpGameClient, Button submitButton)
    {
        _gameEntities = gameEntities;
        _giftCardEntityDictionary = giftCardEntityDictionary;
        _tcpGameClient = tcpGameClient;
        _submitButton = submitButton;
    }

    public void Update(GameData gameData)
    {
        var mouseState = Mouse.GetState();

        if (!gameData.Equals(_lastReceivedGameData))
        {
            _selectedCards.Clear();
            ClearEnlargedEntity();
            ClearSelectedMove();
        }

        _lastReceivedGameData = gameData;

            //if(mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    // tu logika że wybieramy cos
            //}
            //else
            //{
            //    // tu logika ze najezdzamy na cos
            //    foreach
            //}

            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
        if (mouseState.LeftButton == ButtonState.Pressed && _submitButton.IsPointInsideSprite(mousePosition) && _selectedMove != null)
        {
            var selectedCardIds = _selectedCards.Select(x => x.CardId).ToList();

            // walidacja?
            
            _tcpGameClient.SetNextMoveData(new MoveData
            {
                MoveType = _selectedMove.MoveType,
                GiftCards = gameData.GetAllCards().Where(x => selectedCardIds.Contains(x.CardId)).ToList()
            });

            return;
        }

        switch (_selectedMove?.MoveType)
        {
            case PlayerMoveTypeEnum.Elimination:
                break;
            case PlayerMoveTypeEnum.Secret:
                

                bool isAnyCardPointed = false;
                foreach(var card in gameData.CurrentPlayerData.CardsOnHand)
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
                            ChangeEnlargedEntity(cardEntity);
                        }
                    }
                }

                if (!isAnyCardPointed)
                {
                    ClearEnlargedEntity();
                }

                break;
            case PlayerMoveTypeEnum.DoubleGift:
                break;
            case PlayerMoveTypeEnum.Compromise:
                break;
            case PlayerMoveTypeEnum.DoubleGiftOffer:
                break;
            case PlayerMoveTypeEnum.CompromiseOffer:
                break;
            case PlayerMoveTypeEnum.DoubleGiftResponse:
                break;
            case PlayerMoveTypeEnum.CompromiseResponse:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            foreach (var entity in _gameEntities)
            {
                if (entity.IsPointInsideSprite(new Vector2(mouseState.Position.X, mouseState.Position.Y)))
                {
                    if (entity is MoveCardEntity {IsPlayerMove: true} cardEntity)
                    {
                        ChangeSelectedMove(cardEntity);
                    }
                }
            }
        }
    }

    private void ClearEnlargedEntity()
    {
        _enlargedEntity?.Enlarge(false);
        _enlargedEntity = null;
    }

    private void ClearSelectedMove()
    {
        _selectedMove?.Enlarge(false);
        _selectedMove = null;
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

    private void ChangeEnlargedEntity(GameEntity entityToEnlarge)
    {
        _enlargedEntity?.Enlarge(false);
        _enlargedEntity = entityToEnlarge;
        _enlargedEntity.Enlarge();
    }

    private void ChangeSelectedMove(MoveCardEntity cardEntity)
    {
        _selectedMove?.Enlarge(false);
        _selectedMove = cardEntity;
        cardEntity.Enlarge();
    }
}