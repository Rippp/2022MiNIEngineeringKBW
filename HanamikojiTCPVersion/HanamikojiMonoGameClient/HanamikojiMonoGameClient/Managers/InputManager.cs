using System;
using System.Collections.Generic;
using CommonResources.Game;
using HanamikojiClient;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using HanamikojiMonoGameClient.Managers.Moves;

namespace HanamikojiMonoGameClient.Managers;

public class InputManager
{
    private readonly TcpGameClient _tcpGameClient;
    private readonly List<GameEntity> _gameEntities;
    private readonly Button _submitButton;
    private readonly IDictionary<Guid, GiftCardEntity> _giftCardEntityDictionary;

    private GameData _lastReceivedGameData = null;

    private MoveHandler? _currentMoveHandlerValue;
    private MoveHandler? _currentMoveHandler
    {
        get => _currentMoveHandlerValue;
        set
        {
            _currentMoveHandlerValue?.Clear();
            _currentMoveHandlerValue = value;
        }
    }

    private MoveCardEntity? _selectedMoveCardEntityValue;
    private MoveCardEntity? _selectedMoveCardEntity
    {
        get => _selectedMoveCardEntityValue;
        set
        {
            _selectedMoveCardEntityValue?.Enlarge(false);
            value?.Enlarge();
            _selectedMoveCardEntityValue = value;
        }
    }

    public InputManager(List<GameEntity> gameEntities, IDictionary<Guid, GiftCardEntity>  giftCardEntityDictionary, TcpGameClient tcpGameClient, Button submitButton)
    {
        _gameEntities = gameEntities;
        _giftCardEntityDictionary = giftCardEntityDictionary;
        _tcpGameClient = tcpGameClient;
        _submitButton = submitButton;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        var mousePosition = new Vector2(mouseState.X, mouseState.Y);

        ResetIfGameDataDiffers(gameData);

        if (HandleSubmitClick(gameData, mouseState, mousePosition)) return;

        var clickedMoveCardEntity = GetClickedMoveCardEntity(mouseState);
        if(clickedMoveCardEntity != null) ChangeSelectedMove(clickedMoveCardEntity);

        _currentMoveHandler?.Update(gameData, mouseState);
    }

    private bool HandleSubmitClick(GameData gameData, MouseState mouseState, Vector2 mousePosition)
    {
        if (mouseState.LeftButton == ButtonState.Pressed && _submitButton.IsPointInsideSprite(mousePosition) && _currentMoveHandler != null && _currentMoveHandler.Validate())
        {
            var moveData = _currentMoveHandler.GetMoveData(gameData);
            _tcpGameClient.SetNextMoveData(moveData);
            return true;
        }

        return false;
    }

    private void ResetIfGameDataDiffers(GameData gameData)
    {
        if (!gameData.Equals(_lastReceivedGameData))
        {
            _currentMoveHandler?.Clear();
            _currentMoveHandler = null;
            _selectedMoveCardEntity = null;
            _lastReceivedGameData = gameData;
        }
    }

    private MoveCardEntity? GetClickedMoveCardEntity(MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            foreach (var entity in _gameEntities)
            {
                if (entity.IsPointInsideSprite(new Vector2(mouseState.Position.X, mouseState.Position.Y)))
                {
                    if (entity is MoveCardEntity { IsPlayerMove: true } cardEntity)
                    {
                        return cardEntity;
                    }
                }
            }
        }

        return null;
    }

    private void ChangeSelectedMove(MoveCardEntity cardEntity)
    {
        _selectedMoveCardEntity = cardEntity;

        switch (cardEntity.MoveType)
        {
            case PlayerMoveTypeEnum.Elimination:
                _currentMoveHandler = new EliminationMoveHandler(_giftCardEntityDictionary);
                break;
            case PlayerMoveTypeEnum.Secret:
                _currentMoveHandler = new SecretMoveHandler(_giftCardEntityDictionary);
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}