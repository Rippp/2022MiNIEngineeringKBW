using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient;

public class TableManager
{
    private List<MoveCardEntity> _playerMoves;
    private List<MoveCardEntity> _opponentMoves;

    // GameState _lastGameState

    public TableManager(List<MoveCardEntity> playerMoves, List<MoveCardEntity> opponentMoves)
    {
        _playerMoves = playerMoves;
        _opponentMoves = opponentMoves;
    }

    public void Update(GameData gameData)
    {
        // sprawdź czy się różni od poprzeniego gameData
        // if(!lastGameState.Diff(gameState)) return

        ChangeVisibilityOfPlayerMoves(gameData);
        ChangeVisibilityOfOpponentMoves(gameData);
    }

    private void ChangeVisibilityOfOpponentMoves(GameData gameData)
    {
        foreach (var move in gameData.OtherPlayerData.movesAvailability)
        {
            var moveType = move.Key;
            var isMoveAvailable = move.Value;

            var relatedMoveCard = _opponentMoves.Single(x => x.MoveType == moveType);

            if (isMoveAvailable)
            {
                relatedMoveCard.MoveTo(MoveCardPositions.OpponentMoveCardDefaultPositionDictionary[moveType]);
            }
            else
            {
                relatedMoveCard.Hide();
            }
        }
    }

    private void ChangeVisibilityOfPlayerMoves(GameData gameData)
    {
        foreach (var move in gameData.CurrentPlayerData.movesAvailability)
        {
            var moveType = move.Key;
            var isMoveAvailable = move.Value;

            var relatedMoveCard = _playerMoves.Single(x => x.MoveType == moveType);

            if (isMoveAvailable)
            {
                relatedMoveCard.MoveTo(MoveCardPositions.PlayerMoveCardDefaultPositionDictionary[moveType]);
            }
            else
            {
                relatedMoveCard.Hide();
            }
        }
    }
}


public static class MoveCardPositions
{
    public static IDictionary<PlayerMoveTypeEnum, Vector2> PlayerMoveCardDefaultPositionDictionary =
        new Dictionary<PlayerMoveTypeEnum, Vector2>
        {
            {PlayerMoveTypeEnum.Compromise, new Vector2(1000,750)},
            {PlayerMoveTypeEnum.DoubleGift, new Vector2(1120,750)},
            {PlayerMoveTypeEnum.Elimination, new Vector2(1240,750)},
            {PlayerMoveTypeEnum.Secret, new Vector2(1360,750)},
        };

    public static IDictionary<PlayerMoveTypeEnum, Vector2> OpponentMoveCardDefaultPositionDictionary =
        new Dictionary<PlayerMoveTypeEnum, Vector2>
        {
            {PlayerMoveTypeEnum.Compromise, new Vector2(100,100)},
            {PlayerMoveTypeEnum.DoubleGift, new Vector2(220,100)},
            {PlayerMoveTypeEnum.Elimination, new Vector2(340,100)},
            {PlayerMoveTypeEnum.Secret, new Vector2(460,100)},
        };

}