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
            return $"{Type.ToString()}";
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                var giftCard = (GiftCard) obj;
                return giftCard.Type == Type && giftCard.CardId == CardId;
            }
        }

        public string SerializeToJson() => JsonConvert.SerializeObject(this);

        public static GiftCard DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<GiftCard>(jsonData);

        // TODO: can be unit tested
        public static bool AreCardListsTheSame(List<GiftCard>? firstGiftCards, List<GiftCard>? secondGiftCards)
        {
            if (firstGiftCards == null && secondGiftCards == null) return true;
            if (firstGiftCards == null || secondGiftCards == null) return false;
            if (firstGiftCards.Count != secondGiftCards.Count) return false;
            if (firstGiftCards.DistinctBy(x => x.CardId).Count() != firstGiftCards.Count)
                throw new Exception("There are duplicated card ids");
            if (secondGiftCards.DistinctBy(x => x.CardId).Count() != secondGiftCards.Count)
                throw new Exception("There are duplicated card ids");
            if (!firstGiftCards.TrueForAll(x => secondGiftCards.Exists(y => y.CardId == x.CardId && y.Equals(x)))) return false;
            return true;
        }
    }
}
