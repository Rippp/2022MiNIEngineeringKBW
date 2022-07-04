using HanamikojiConsoleVersion.GameControl;

namespace HanamikojiConsoleVersion.Entities.Moves;

public class EliminationMove : IPlayerMove
{
    public List<GiftCard> EliminatedCards { get; private set; }

    public EliminationMove(List<GiftCard> eliminatedCards)
    {
        EliminatedCards = eliminatedCards;
    }

    public void Execute(Referee referee) => referee.Execute(this);

    public override string ToString() => nameof(EliminationMove);
}