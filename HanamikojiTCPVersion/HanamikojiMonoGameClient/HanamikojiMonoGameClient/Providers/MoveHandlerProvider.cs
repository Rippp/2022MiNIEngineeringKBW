using System;
using CommonResources.Game;
using HanamikojiMonoGameClient.Managers.Moves;

namespace HanamikojiMonoGameClient.Providers;

public class MoveHandlerProvider : IMoveHandlerProvider
{
    private readonly SecretMoveHandler _secretMoveHandler;
    private readonly EliminationMoveHandler _eliminationMoveHandler;

    public MoveHandlerProvider(SecretMoveHandler secretMoveHandler, EliminationMoveHandler eliminationMoveHandler)
    {
        _secretMoveHandler = secretMoveHandler;
        _eliminationMoveHandler = eliminationMoveHandler;
    }

    public MoveHandler GetMoveHandler(PlayerMoveTypeEnum moveType)
    {
        switch (moveType)
        {
            case PlayerMoveTypeEnum.Elimination:
                return _eliminationMoveHandler;
            case PlayerMoveTypeEnum.Secret:
                return _secretMoveHandler;
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

        return null;
    }
}

public interface IMoveHandlerProvider
{
    public MoveHandler GetMoveHandler(PlayerMoveTypeEnum moveType);
}
