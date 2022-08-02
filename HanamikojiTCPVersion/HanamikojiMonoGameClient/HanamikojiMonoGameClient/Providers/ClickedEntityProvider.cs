using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Providers;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace HanamikojiMonoGameClient.Managers;

public class ClickedEntityProvider
{
    private readonly PointedEntityProvider _pointedEntityProvider;

    private GiftCardEntity? _clickedCardOnHandEntity = null;
    private List<GiftCardEntity>? _clickedDoubleGiftOfferCardEntities = null;

    public ClickedEntityProvider(PointedEntityProvider pointedEntityProvider)
    {
        _pointedEntityProvider = pointedEntityProvider;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        if(gameData.CurrentPossibleMoves.Contains(PlayerMoveTypeEnum.DoubleGiftOffer))
            UpdateClickedDoubleGiftOfferCards(gameData, mouseState);
        else
            UpdateClickedCardOnHand(gameData, mouseState);
    }

    private void UpdateClickedCardOnHand(GameData gameData, MouseState mouseState)
    {
        var pointedCardEntity = _pointedEntityProvider.GetPointedCardOnHand(gameData, mouseState);
        if (_clickedCardOnHandEntity == null && pointedCardEntity != null && mouseState.LeftButton == ButtonState.Pressed)
        {
            _clickedCardOnHandEntity = pointedCardEntity;
        }
        else if (pointedCardEntity == null || mouseState.LeftButton == ButtonState.Released)
        {
            _clickedCardOnHandEntity = null;
        }
    }
   
    private void UpdateClickedDoubleGiftOfferCards(GameData gameData, MouseState mouseState)
    {
        var pointedDoubleGiftOfferEntities = _pointedEntityProvider.GetPointedDoubleGiftOfferCards(gameData, mouseState);
        if(pointedDoubleGiftOfferEntities != null && _clickedDoubleGiftOfferCardEntities == null && mouseState.LeftButton == ButtonState.Pressed)
        {
            _clickedDoubleGiftOfferCardEntities = pointedDoubleGiftOfferEntities;
        }
        else if(pointedDoubleGiftOfferEntities == null || mouseState.LeftButton == ButtonState.Released)
        {
            _clickedDoubleGiftOfferCardEntities = null;
        }
    }
    
    public GiftCardEntity? GetClickedCardOnHandEntity() => _clickedCardOnHandEntity;
    public List<GiftCardEntity>? GetClickedDoubleGiftOfferCardEntities() => _clickedDoubleGiftOfferCardEntities;
}