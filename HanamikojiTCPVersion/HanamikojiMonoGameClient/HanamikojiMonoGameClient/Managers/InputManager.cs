using System;
using System.Collections.Generic;
using CommonResources.Game;
using HanamikojiClient;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using HanamikojiMonoGameClient.Managers.Moves;
using HanamikojiMonoGameClient.Providers;

namespace HanamikojiMonoGameClient.Managers;

public class InputManager
{
    private ITcpGameClientProvider _tcpClientProvider;
    private IEntitiesRepository _entitiesRepository;
    private IMoveHandlerProvider _moveHandlerProvider;

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

    public InputManager(IEntitiesRepository entitiesRepository, IMoveHandlerProvider moveHandlerProvider, ITcpGameClientProvider tcpClientProvider)
    {
        _entitiesRepository = entitiesRepository;
        _moveHandlerProvider = moveHandlerProvider;
        _tcpClientProvider = tcpClientProvider;
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
        if (mouseState.LeftButton == ButtonState.Pressed &&
            _entitiesRepository.GetSubmitButton().IsPointInsideSprite(mousePosition) && 
            _currentMoveHandler != null 
            && _currentMoveHandler.Validate())
        {
            var moveData = _currentMoveHandler.GetMoveData(gameData);
            _tcpClientProvider.GetTcpGameClient().SetNextMoveData(moveData);
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
            foreach (var entity in _entitiesRepository.GetAll())
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
        _currentMoveHandler = _moveHandlerProvider.GetMoveHandler(cardEntity.MoveType);
    }
}