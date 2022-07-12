using HanamikojiConsoleVersion.GameControl;

namespace HanamikojiConsoleVersion.Entities.Moves;

public class CompromiseMove : IPlayerMove
{
    public List<GiftCard> CardToChooseFrom { get; private set; }

    public CompromiseMove(List<GiftCard> cardToChooseFrom)
    {
        CardToChooseFrom = cardToChooseFrom;
    }

    public void Execute(Referee referee)
    {
        referee.Execute(this);
    }

    public override string ToString() => nameof(CompromiseMove);
}
