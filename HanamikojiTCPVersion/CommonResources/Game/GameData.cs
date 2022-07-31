using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class GameData
    {
        [JsonProperty]
        public PlayerData CurrentPlayerData { get; private set; }
        [JsonProperty]
        public PlayerData OtherPlayerData { get; private set; }
        [JsonProperty]
        public List<PlayerMoveTypeEnum> MovesAvailable { get; private set; } = new List<PlayerMoveTypeEnum>();
        [JsonProperty]
        public List<GiftCard>? CompromiseCards { get; private set; }
        [JsonProperty]
        public List<GiftCard>? DoubleGiftCards { get; private set; }
        [JsonProperty]
        public string MessageToCurrentPlayer { get; private set; }

        public GameData(PlayerData currentPlayerData, PlayerData otherPlayerData,
            List<PlayerMoveTypeEnum>? movesAvailable = null, string messageToCurrentPlayer = "")
        {
            CurrentPlayerData = currentPlayerData;
            OtherPlayerData = otherPlayerData;
            MovesAvailable = movesAvailable ?? currentPlayerData.GetAvailableMoves();
            MessageToCurrentPlayer = messageToCurrentPlayer;
        }

        public void SetCompromiseCards(List<GiftCard> compromiseCards) =>
            CompromiseCards = new List<GiftCard>(compromiseCards);

        public void SetDoubleGiftCards(List<GiftCard> doubleGiftCards) =>
            DoubleGiftCards = new List<GiftCard>(doubleGiftCards);

        public string SerializeToJson()
            => JsonConvert.SerializeObject(this);

        public static GameData DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<GameData>(jsonData);

        public override bool Equals(object? obj)
        {
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (this == obj) return true;

            var gameData = (GameData)obj;
                
            return gameData.CurrentPlayerData.Equals(CurrentPlayerData) &&
                   gameData.OtherPlayerData.Equals(OtherPlayerData) && 
                   gameData.MovesAvailable.Equals(MovesAvailable) &&
                   GiftCard.AreCardListsTheSame(gameData.CompromiseCards, CompromiseCards) &&
                   GiftCard.AreCardListsTheSame(gameData.DoubleGiftCards, DoubleGiftCards);
        }

        public IReadOnlyList<GiftCard> GetAllCards()
        {
            var allCards = new List<GiftCard>();

            allCards.AddRange(CurrentPlayerData.AllCardsInPlayerData());
            allCards.AddRange(this.OtherPlayerData.AllCardsInPlayerData());
            if (CompromiseCards != null) allCards.AddRange(CompromiseCards);
            if (DoubleGiftCards != null) allCards.AddRange(DoubleGiftCards);

            return allCards;
        }
    }
}
