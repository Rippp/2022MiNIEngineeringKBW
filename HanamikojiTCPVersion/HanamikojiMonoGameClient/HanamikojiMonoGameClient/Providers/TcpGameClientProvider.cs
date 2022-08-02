using HanamikojiClient;

namespace HanamikojiMonoGameClient.Providers;

public interface ITcpGameClientProvider
{
    public TcpGameClient GetTcpGameClient();
}

public class TcpGameClientProvider : ITcpGameClientProvider
{
    private static TcpGameClient _tcpGameClient;
    private const string _host = "localhost";
    private const int _port = 6000;

    public TcpGameClientProvider()
    {
        _tcpGameClient = new TcpGameClient(_host, _port);
    }

    public TcpGameClient GetTcpGameClient()
    {
        return _tcpGameClient;
    }
}