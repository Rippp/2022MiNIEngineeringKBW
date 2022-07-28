using System;

namespace HanamikojiMonoGameClient
{
    public static class Program
    {
        private const string _host = "localhost";
        private const int _port = 6000;

        [STAThread]
        static void Main()
        {
            var gameClient = new TcpGameClient(_host, _port);
            using var game = new MonoGameClient(gameClient);
            game.Run();
        }
    }
}