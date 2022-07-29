using System;
using System.Collections.Generic;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.Managers;

public class MoveAnimationManager
{
    private readonly IDictionary<GiftCardEntity, AnimationEntry>
        _currentPlayingAnimationCardDestinations
            = new Dictionary<GiftCardEntity, AnimationEntry>();

    public void AddMoveAnimationToDestination(GiftCardEntity giftCard, Vector2 destination)
    {
        _currentPlayingAnimationCardDestinations[giftCard] = new AnimationEntry(giftCard.Position, destination, GameSettings.DEFAULT_ANIMATION_DURATION);
    }

    public void Update(GameTime gameTime)
    {
        foreach (var giftCard in _currentPlayingAnimationCardDestinations.Keys)
        {
            var animationEntry = _currentPlayingAnimationCardDestinations[giftCard];


            //giftCard.MoveToPosition(animationEntry.Destination);
            //_currentPlayingAnimationCardDestinations.Remove(giftCard);


            var isEndOfAnimation = animationEntry.Update(gameTime.ElapsedGameTime);
            giftCard.MoveToPosition(animationEntry.CurrentPosition);
            if (isEndOfAnimation) _currentPlayingAnimationCardDestinations.Remove(giftCard);
        }
    }
}


public record AnimationEntry
{
    public AnimationEntry(Vector2 start, Vector2 destination, TimeSpan duration)
    {
        Start = start;
        Destination = destination;
        Duration = duration;
        _progress = 0;
        _timePassed = TimeSpan.Zero;
    }

    public Vector2 Start { get; private set; }
    public Vector2 Destination { get; private set; }
    public TimeSpan Duration { get; set; }

    private float _progress;
    public float Progress => EaseInOut(_progress);

    private TimeSpan _timePassed;

    public Vector2 CurrentPosition => Vector2.Lerp(Start, Destination, Progress);

    public bool Update(TimeSpan elapsed)
    {
        _timePassed += elapsed;
        _progress = (float)Math.Min(_timePassed.TotalMilliseconds / Duration.TotalMilliseconds, 1);
        return _timePassed >= Duration;
    }

    static public float EaseInOut(float t)
    {
        return t * t * (3.0f - 2.0f * t);
    }
}


