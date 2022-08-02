using System;
using CommonResources.Game;
using HanamikojiMonoGameClient.Managers.Moves;

namespace HanamikojiMonoGameClient.Providers;

public class MoveHandlerProvider : IMoveHandlerProvider
{
    private readonly SecretMoveHandler _secretMoveHandler;
    private readonly EliminationMoveHandler _eliminationMoveHandler;
    private readonly DoubleGiftMoveHandler _doubleGiftMoveHandler;
    private readonly DoubleGiftOfferResponseMoveHandler _doubleGiftOfferMoveHandler;

    public MoveHandlerProvider(
        SecretMoveHandler secretMoveHandler, 
        EliminationMoveHandler eliminationMoveHandler, 
        DoubleGiftMoveHandler doubleGiftMoveHandler,
        DoubleGiftOfferResponseMoveHandler doubleGiftOfferMoveHandler)
    {
        _secretMoveHandler = secretMoveHandler;
        _eliminationMoveHandler = eliminationMoveHandler;
        _doubleGiftMoveHandler = doubleGiftMoveHandler;
        _doubleGiftOfferMoveHandler = doubleGiftOfferMoveHandler;
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
                return _doubleGiftMoveHandler;
            case PlayerMoveTypeEnum.Compromise:
                break;
            case PlayerMoveTypeEnum.DoubleGiftOffer:
                return _doubleGiftOfferMoveHandler;
            case PlayerMoveTypeEnum.CompromiseOffer:
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
