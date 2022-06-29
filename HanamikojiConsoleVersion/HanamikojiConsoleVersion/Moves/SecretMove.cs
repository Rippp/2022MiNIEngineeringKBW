namespace HanamikojiConsoleVersion.Moves;

public class SecretMove : IPlayerMove
{
    public GiftCard SecretCard { get; private set; }

    public SecretMove(GiftCard secretCard)
    {
        SecretCard = secretCard;
    }

    public void Execute(Player player, Referee referee)
    {
        referee.Execute(player, this);
    }
}