using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class MoveData
    {
        public PlayerMoveTypeEnum MoveType { get; set; }
        public List<GiftCard> GiftCards { get; set; } = new List<GiftCard>();
        public List<GiftCard>? AnswerGiftCards { get; set; } = null;

        public string SerializeToJson()
            => JsonConvert.SerializeObject(this);

        public static MoveData DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<MoveData>(jsonData);
    }
}
