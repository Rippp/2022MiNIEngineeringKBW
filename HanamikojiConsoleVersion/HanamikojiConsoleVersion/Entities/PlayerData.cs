namespace HanamikojiConsoleVersion.Entities;

public class PlayerData
{
    public List<GiftCard> CardsOnHand { get; set; } = new List<GiftCard>();
    public List<GiftCard> GiftsFromPlayer { get; set; } = new List<GiftCard>();
    public GiftCard? SecretCard { get; set; } = null;
}
