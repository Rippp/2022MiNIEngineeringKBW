using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class GiftCard
    {
        public GeishaType Type { get; set; }

        public GiftCard(GeishaType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public string SerializeToJson() => JsonConvert.SerializeObject(this);

        public static GiftCard DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<GiftCard>(jsonData);
    }
}
