using System;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient;

public interface IGame : IDisposable
{
    Game Game { get; }

    // include all the Properties/Methods that you'd want to use on your Game class below.
    GameWindow Window { get; }

    event EventHandler<EventArgs> Exiting;

    void Run();
    void Exit();
}