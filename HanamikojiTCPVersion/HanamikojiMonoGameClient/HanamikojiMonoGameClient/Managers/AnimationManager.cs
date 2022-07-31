using System;
using System.Collections.Generic;
using System.Linq;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.Managers;

public class AnimationManager
{
    private readonly IDictionary<GiftCardEntity, MoveAnimationEntry>
        _currentPlayingMoveAnimations
            = new Dictionary<GiftCardEntity, MoveAnimationEntry>();

    private readonly IDictionary<GiftCardEntity, RotationAnimationEntry> 
        _currentRotationAnimationEntries 
            = new Dictionary<GiftCardEntity, RotationAnimationEntry>();


    public void AddMoveAnimationToDestination(GiftCardEntity giftCard, Vector2 destination)
    {
        _currentPlayingMoveAnimations[giftCard] = new MoveAnimationEntry(giftCard.Position, destination, GameSettings.DEFAULT_ANIMATION_DURATION);
    }

    public void AddRotationAnimation(GiftCardEntity giftCard, float targetRotation)
    {
        _currentRotationAnimationEntries[giftCard] = new RotationAnimationEntry(giftCard.Rotation, targetRotation, GameSettings.DEFAULT_ANIMATION_DURATION);
    }



    public void Update(GameTime gameTime)
    {
        UpdateMoveAnimations(gameTime);
        UpdateRotationAnimations(gameTime);

    }

    private void UpdateRotationAnimations(GameTime gameTime)
    {
        var cardEntitiesOfEndedAnimations = new List<GiftCardEntity>();
        foreach (var giftCard in _currentRotationAnimationEntries.Keys)
        {
            var animationEntry = _currentRotationAnimationEntries[giftCard];
            var isEndOfAnimation = animationEntry.Update(gameTime.ElapsedGameTime);

            giftCard.SetRotation(animationEntry.CurrentRotation);
            if (isEndOfAnimation)
            {
                cardEntitiesOfEndedAnimations.Add(giftCard);
            }
        }

        cardEntitiesOfEndedAnimations.ForEach(x => _currentRotationAnimationEntries.Remove(x));
    }

    private void UpdateMoveAnimations(GameTime gameTime)
    {
        var cardEntitiesOfEndedAnimations = new List<GiftCardEntity>();
        foreach (var giftCard in _currentPlayingMoveAnimations.Keys)
        {
            var animationEntry = _currentPlayingMoveAnimations[giftCard];
            var isEndOfAnimation = animationEntry.Update(gameTime.ElapsedGameTime);

            giftCard.MoveToPosition(animationEntry.CurrentPosition);
            if (isEndOfAnimation)
            {
                cardEntitiesOfEndedAnimations.Add(giftCard);
            }
        }

        cardEntitiesOfEndedAnimations.ForEach(x => _currentPlayingMoveAnimations.Remove(x));
    }
}


public abstract record AnimationEntry
{
    public TimeSpan Duration { get; set; }
    public float Progress => _isLinear ? _progress : EaseInOut(_progress);
    protected bool _isLinear { get; init; }
    protected float _progress = 0;
    protected TimeSpan _timePassed = TimeSpan.Zero;

    public bool Update(TimeSpan elapsed)
    {
        _timePassed += elapsed;
        _progress = (float)Math.Min(_timePassed.TotalMilliseconds / Duration.TotalMilliseconds, 1);
        return _timePassed >= Duration;
    }

    public static float EaseInOut(float t)
    {
        return t * t * (3.0f - 2.0f * t);
    }
}


public record RotationAnimationEntry : AnimationEntry
{
    public float StartRotation { get; init; }
    public float TargetRotation { get; init; }
    public float CurrentRotation => StartRotation + (TargetRotation - StartRotation) * Progress;

    public RotationAnimationEntry(float startRotation, float targetRotation, TimeSpan duration, bool isLinear = false)
    {
        StartRotation = startRotation;
        TargetRotation = targetRotation;
        Duration = duration;
        _isLinear = isLinear;
    }
}

public record MoveAnimationEntry : AnimationEntry
{
    public Vector2 Start { get; private set; }
    public Vector2 Destination { get; private set; }
    public Vector2 CurrentPosition => Vector2.Lerp(Start, Destination, Progress);

    public MoveAnimationEntry(Vector2 start, Vector2 destination, TimeSpan duration, bool isLinear = false)
    {
        Start = start;
        Destination = destination;
        Duration = duration;
        _isLinear = isLinear;
    }
}


