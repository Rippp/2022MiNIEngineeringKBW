namespace HanamikojiConsoleVersion.Moves;

public class EliminateMove : IPlayerMove
{
    public GiftCard EliminatedCard { get; init; }

    public EliminateMove(GiftCard eliminatedCard)
    {
        EliminatedCard = eliminatedCard;
    }

    public void Execute(Player player, Referee referee)
    {
        throw new NotImplementedException();
    }
}
