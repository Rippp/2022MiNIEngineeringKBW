using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class PlayerData
    {
        public List<GiftCard> CardsOnHand { get; set; } = new List<GiftCard>();
        public List<GiftCard> GiftsFromPlayer { get; set; } = new List<GiftCard>();
        public GiftCard? SecretCard { get; set; } = null;
        public List<GiftCard>? EliminationCards { get; set; } = null;
        public List<GeishaType> ConvincedGeishasInPreviousRound = new List<GeishaType>();

        public Dictionary<PlayerMoveTypeEnum, bool> movesAvailability = new()
        {
            {PlayerMoveTypeEnum.DoubleGift, true},
            {PlayerMoveTypeEnum.Secret, true},
            {PlayerMoveTypeEnum.Elimination, true},
            {PlayerMoveTypeEnum.Compromise, true}
        };

        public void ClearData()
        {
            CardsOnHand.Clear();
            GiftsFromPlayer.Clear();
            SecretCard = null;
            EliminationCards = null;
            ConvincedGeishasInPreviousRound.Clear();
            MakeAllMovesAvailable();
        }

        public List<PlayerMoveTypeEnum> GetAvailableMoves() => movesAvailability.Where(x => x.Value).Select(x => x.Key).ToList();

        public bool IsMoveAvailable(PlayerMoveTypeEnum move) => movesAvailability[move];

        public void MarkMoveAsNotAvailable(PlayerMoveTypeEnum move) => movesAvailability[move] = false;

        public bool IsAnyMoveAvailable() => movesAvailability.Any(x => x.Value);

        public string SerializeToJson()
            => JsonConvert.SerializeObject(this);

        public static PlayerData DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<PlayerData>(jsonData);

        public PlayerData AnonimizeData()
        {
            var anonimizedData = new PlayerData();

            anonimizedData.CardsOnHand = CardsOnHand.Select(x => new GiftCard(GeishaType.AnonimizedGeisha, x.CardId)).ToList();
            anonimizedData.GiftsFromPlayer = GiftsFromPlayer;
            anonimizedData.SecretCard = SecretCard == null ? null : new GiftCard(GeishaType.AnonimizedGeisha, SecretCard.CardId);
            anonimizedData.EliminationCards = EliminationCards == null ?
                null : EliminationCards.Select(x => new GiftCard(GeishaType.AnonimizedGeisha, x.CardId)).ToList();

            return anonimizedData;
        }

        public int CountPointsForGeishaType(GeishaType geishaType) =>
            GiftsFromPlayer.Count(card => card.Type == geishaType);

        private void MakeAllMovesAvailable()
        {
            foreach (var move in movesAvailability.Keys) movesAvailability[move] = true;
        }
    }
}
