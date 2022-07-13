using Newtonsoft.Json;

namespace Infrastructure.Entities
{
    public class PlayerData
    {
        public List<GiftCard> GiftCards { get; set; } = new List<GiftCard>()
        {
            new GiftCard(GeishaType.Geisha2_A),
            new GiftCard(GeishaType.Geisha2_B),
            new GiftCard(GeishaType.Geisha2_C),
        };

        public string SerializeToJson() 
            => JsonConvert.SerializeObject(this);

        public static PlayerData DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<PlayerData>(jsonData);
    }
}
