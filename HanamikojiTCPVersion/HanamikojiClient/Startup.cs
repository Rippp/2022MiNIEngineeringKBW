namespace HanamikojiClient;

public class Startup
{
    public static TcpGameClient gameClient;
    private const string _host = "localhost";
    private const int _port = 6000;

    public static void GracefulDisconnectClient(object sender, ConsoleCancelEventArgs args)
    {
        args.Cancel = true;
        gameClient?.Disconnect();
    }

    public static void Main()
    {
        gameClient = new TcpGameClient(_host, _port);

        Console.CancelKeyPress += GracefulDisconnectClient;

        gameClient.ConnectToServer();
        gameClient.Run();
    }
}
