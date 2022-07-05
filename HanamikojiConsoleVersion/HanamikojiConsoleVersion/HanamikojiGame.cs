﻿using HanamikojiConsoleVersion.Entities;
using HanamikojiConsoleVersion.GameControl;
using HanamikojiConsoleVersion.InputUI;

namespace HanamikojiConsoleVersion;

public static class HanamikojiGame
{
    public static void RunGame()
    {
        var p1 = new Player("Krzysztof");
        var p2 = new Player("Adam");
        var referee = new Referee(p1, p2);

        while (!referee.NextRound())
        {
        }
    }
}