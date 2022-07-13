using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class PlayerData
    {
        public List<GiftCard> CardsOnHand { get; set; } = new List<GiftCard>();

        public void ClearData() => CardsOnHand.Clear();

        public string SerializeToJson()
            => JsonConvert.SerializeObject(this);

        public static PlayerData DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<PlayerData>(jsonData);
    }
}
