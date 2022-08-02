using System;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.Animations.Entries;

public record MoveAnimationEntry : BaseAnimationEntry
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