using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class GameData
    {
        public PlayerData CurrentPlayerData { get; private set; }
        public PlayerData OtherPlayerData { get; private set; }
        public List<PlayerMoveTypeEnum> MovesAvailable { get; private set; } = new List<PlayerMoveTypeEnum>();
        public List<GiftCard>? CompromiseCards { get; private set; }
        public List<GiftCard>? DoubleGiftCards { get; private set; }

        public GameData(PlayerData currentPlayerData, PlayerData otherPlayerData, List<PlayerMoveTypeEnum>? movesAvailable = null)
        {
            CurrentPlayerData = currentPlayerData;
            OtherPlayerData = otherPlayerData;
            MovesAvailable = movesAvailable ?? currentPlayerData.GetAvailableMoves();
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
