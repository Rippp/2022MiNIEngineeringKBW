using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class GiftCard
    {
        public GeishaType Type { get; set; }
        public Guid CardId { get; private set; }

        public GiftCard(GeishaType type, Guid cardId)
        {
            Type = type;
            CardId = cardId;
        }

        public override string ToString()
        {
            return $"{Type.ToString()} - {CardId}";
        }

        public string SerializeToJson() => JsonConvert.SerializeObject(this);

        public static GiftCard DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<GiftCard>(jsonData);
    }
}
