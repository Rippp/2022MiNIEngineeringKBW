using System.Collections.Generic;
using HanamikojiMonoGameClient.Animations.Entries;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.Animations;

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








