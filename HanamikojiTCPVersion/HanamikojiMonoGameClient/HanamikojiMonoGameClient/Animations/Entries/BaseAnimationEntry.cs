using System;

namespace HanamikojiMonoGameClient.Animations.Entries;

public abstract record BaseAnimationEntry
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