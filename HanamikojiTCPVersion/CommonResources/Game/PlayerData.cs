using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class PlayerData
    {
        public List<GiftCard> CardsOnHand { get; private set; } = new List<GiftCard>();
        public List<GiftCard> GiftsFromPlayer { get; private set; } = new List<GiftCard>();
        public GiftCard? SecretCard { get; private set; } = null;
        public List<GiftCard>? EliminationCards { get; private set; } = null;
        public List<GeishaType> ConvincedGeishasInPreviousRound { get; private set; }  = new List<GeishaType>();

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

        public void MarkMoveAsNotAvailable(PlayerMoveTypeEnum move)
        {
            if (movesAvailability.ContainsKey(move)) movesAvailability[move] = false;
        }

        public bool IsAnyMoveAvailable() => movesAvailability.Any(x => x.Value);

        public void AddSecretCard(GiftCard secretCard)
        {
            if (SecretCard != null) throw new Exception("Secret card is already set");
            SecretCard = secretCard;
        }

        public void AddEliminationCards(List<GiftCard> eliminationCards)
        {
            if (EliminationCards != null) throw new Exception("Elimination cards are already set");
            EliminationCards = eliminationCards.ToList();
        }

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
            anonimizedData.movesAvailability = new Dictionary<PlayerMoveTypeEnum, bool>(movesAvailability);

            return anonimizedData;
        }

        public int CountPointsForGeishaType(GeishaType geishaType) =>
            GiftsFromPlayer.Count(card => card.Type == geishaType);

        public IReadOnlyList<GiftCard> AllCardsInPlayerData()
        {
            var allCards = new List<GiftCard>();

            allCards.AddRange(CardsOnHand);
            allCards.AddRange(GiftsFromPlayer);
            if(SecretCard != null) allCards.Add(SecretCard);
            if(EliminationCards != null) allCards.AddRange(EliminationCards);

            return allCards;
        }

        // TODO: can be unit tested
        public override bool Equals(object? obj)
        {
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }

            var playerData = (PlayerData)obj;

            return GiftCard.AreCardListsTheSame(CardsOnHand, playerData.CardsOnHand) &&
                   GiftCard.AreCardListsTheSame(GiftsFromPlayer, playerData.GiftsFromPlayer) &&
                   GiftCard.AreCardListsTheSame(EliminationCards, playerData.EliminationCards) &&
                   ((SecretCard == null && playerData.SecretCard == null) || SecretCard.Equals(playerData.SecretCard)) &&
                   movesAvailability.Equals(playerData.movesAvailability) &&
                   AreConvincedGeishaInPreviousRoundAreTheSame(playerData.ConvincedGeishasInPreviousRound);
        }

        private bool AreConvincedGeishaInPreviousRoundAreTheSame(List<GeishaType> geishaTypes)
        {
            if (ConvincedGeishasInPreviousRound == null && geishaTypes == null) return true;
            if (ConvincedGeishasInPreviousRound == null || geishaTypes == null) return false;
            if (ConvincedGeishasInPreviousRound.Count != geishaTypes.Count) return false;
            if (ConvincedGeishasInPreviousRound.Distinct().Count() != geishaTypes.Count)
                throw new Exception("There are duplicated card ids");
            if (geishaTypes.Distinct().Count() != ConvincedGeishasInPreviousRound.Count)
                throw new Exception("There are duplicated card ids");
            if (!ConvincedGeishasInPreviousRound.TrueForAll(geishaTypes.Contains)) return false;
            return true;
        }

        private void MakeAllMovesAvailable()
        {
            foreach (var move in movesAvailability.Keys) movesAvailability[move] = true;
        }
    }
}
