using System;

namespace HanamikojiMonoGameClient
{
    public static class GameSettings
    {
        public static TimeSpan DEFAULT_ANIMATION_DURATION = new TimeSpan(0,0,0,1);
        public static int WINDOW_HEIGHT = 900;
        public static int WINDOW_WIDTH = 1600;
        public static int FPS = 144;

        public static float ENLARGED_MOVE_SCALE = 1.2f;

        public static int MILISECONDS_BETWEEN_ACTIONS = 700;
    }
}
