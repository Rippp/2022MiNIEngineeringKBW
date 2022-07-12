using HanamikojiConsoleVersion.GameControl;

namespace HanamikojiConsoleVersion.Entities.Moves;

public class DoubleGiftMove : IPlayerMove
{
    public GiftCard[] PairOne { get; init; }
    public GiftCard[] PairTwo { get; init; }

    public DoubleGiftMove(GiftCard[] pairOne, GiftCard[] pairTwo)
    {
        PairOne = pairOne;
        PairTwo = pairTwo;
    }

    public void Execute(Referee referee)
    {
        referee.Execute(this);
    }

    public override string ToString() => nameof(DoubleGiftMove);
}