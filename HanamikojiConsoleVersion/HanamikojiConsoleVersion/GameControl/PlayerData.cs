using HanamikojiConsoleVersion.Entities;
using HanamikojiConsoleVersion.Entities.Moves;

namespace HanamikojiConsoleVersion.GameControl;

public class PlayerData
{
    public List<GiftCard> CardsOnHand { get; set; } = new List<GiftCard>();
    public List<GiftCard> GiftsFromPlayer { get; set; } = new List<GiftCard>();
    public GiftCard? SecretCard { get; set; } = null;
    public List<GiftCard>? EliminatedCards { get; set; } = null;

    private readonly Dictionary<string, bool> _movesAvailability = new()
    {
        {nameof(CompromiseMove), true},
        {nameof(EliminationMove), true },
        {nameof(SecretMove), true },
        {nameof(DoubleGiftMove), true }
    };

    public List<string> GetAvailableMoves() 
        => _movesAvailability.Where(x => x.Value).Select(x => x.Key).ToList();
    public bool IsMoveAvailable(string move) => _movesAvailability[move];

    public void MarkMoveAsNotAvailable(string move) => _movesAvailability[move] = false;

    public bool IsAnyMoveAvailable() => _movesAvailability.Any(x => x.Value);

    public void ClearData()
    {
        CardsOnHand.Clear();
        GiftsFromPlayer.Clear();
        SecretCard = null;
        EliminatedCards = null;
        _makeAllMovesAvailable();
    }

    public int CountPointsForGeishaType(GeishaType geishaType) =>
        GiftsFromPlayer.Count(card => card.Type == geishaType);

    private void _makeAllMovesAvailable()
    {
        foreach (var move in _movesAvailability.Keys) _movesAvailability[move] = true;
    }
}
