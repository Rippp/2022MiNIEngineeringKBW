using System;
using HanamikojiMonoGameClient.Managers;

namespace HanamikojiMonoGameClient.Animations.Entries;

public record RotationAnimationEntry : BaseAnimationEntry
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