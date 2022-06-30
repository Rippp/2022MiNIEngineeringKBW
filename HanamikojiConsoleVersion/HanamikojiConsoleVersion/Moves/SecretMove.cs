namespace HanamikojiConsoleVersion.Moves;

public class SecretMove : IPlayerMove
{
    public GiftCard SecretCard { get; private set; }

    public SecretMove(GiftCard secretCard)
    {
        SecretCard = secretCard;
    }

    public void Execute(Referee referee)
    {
        referee.Execute(this);
    }
}