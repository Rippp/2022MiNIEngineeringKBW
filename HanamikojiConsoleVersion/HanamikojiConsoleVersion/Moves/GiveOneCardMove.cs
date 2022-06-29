namespace HanamikojiConsoleVersion.Moves;

public class GiveOneCardMove : IPlayerMove
{
    public GiftCard CardToGive { get; private set; }

    public GiveOneCardMove(GiftCard cardToGift)
    {
        CardToGive = cardToGift;
    }

    public void Execute(Player player, Referee referee)
    {
        referee.Execute(player, this);
    }
}
