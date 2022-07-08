namespace Server;

public class Startup
{
    public static TcpGameServer gamesServer;

    public static void GracefulServerShutdown(object sender, ConsoleCancelEventArgs args)
    {
        args.Cancel = true;
        gamesServer?.Shutdown();
    }

    public static void Main(string[] args)
    {
        int serverPort = 6000;

        Console.CancelKeyPress += GracefulServerShutdown;

        gamesServer = new TcpGameServer(serverPort);
        gamesServer.Run();
    }
}
