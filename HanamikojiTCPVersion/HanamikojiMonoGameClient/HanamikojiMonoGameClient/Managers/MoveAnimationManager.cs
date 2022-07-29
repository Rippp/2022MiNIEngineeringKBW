using System.Collections.Generic;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient;

public class MoveAnimationManager
{
    private readonly IDictionary<GiftCardEntity, Vector2> _currentPlayingAnimationCardDestinations 
        = new Dictionary<GiftCardEntity, Vector2>();

    public void AddMoveAnimationToDestination(GiftCardEntity giftCard, Vector2 destination)
    {
        _currentPlayingAnimationCardDestinations[giftCard] = destination;
    }

    public void Update(GameTime gameTime)
    {
        var elapsedMilliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;

        foreach (var giftCard in _currentPlayingAnimationCardDestinations.Keys)
        {
            var destination = _currentPlayingAnimationCardDestinations[giftCard];

            // TODO: przesuwaj klatka po klatce, zanim pozycja giftCard != destinaion, jak się równa to usuń z kolekcji
            giftCard.MoveToPosition(destination);
            _currentPlayingAnimationCardDestinations.Remove(giftCard);
        }
    }
}