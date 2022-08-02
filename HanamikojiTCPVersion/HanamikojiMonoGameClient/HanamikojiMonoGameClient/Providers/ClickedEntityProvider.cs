using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Providers;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient.Managers;

public class ClickedEntityProvider
{
    private readonly IPointedEntityProvider _pointedEntityProvider;

    private GiftCardEntity? _clickedCardEntity = null;

    public ClickedEntityProvider(IPointedEntityProvider pointedEntityProvider)
    {
        _pointedEntityProvider = pointedEntityProvider;
    }

    public void Update(GameData gameData, MouseState mouseState)
    {
        var pointedCardEntity = _pointedEntityProvider.GetPointedCardOnHand(gameData, mouseState);
        if (_clickedCardEntity == null && pointedCardEntity != null && mouseState.LeftButton == ButtonState.Pressed)
        {
            _clickedCardEntity = pointedCardEntity;
        }
        else if(pointedCardEntity == null || mouseState.LeftButton == ButtonState.Released)
        {
            _clickedCardEntity = null;
        }
    }
    public GiftCardEntity? GetClickedCardOnHandEntity() => _clickedCardEntity;
}