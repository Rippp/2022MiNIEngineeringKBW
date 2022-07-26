﻿using Newtonsoft.Json;

namespace CommonResources.Game
{
    public class GameData
    {
        public PlayerData CurrentPlayerData { get; set; }
        public PlayerData OtherPlayerData { get; set; }
        public List<PlayerMoveTypeEnum> MovesAvailable { get; set; } = new List<PlayerMoveTypeEnum>();
        public List<GiftCard>? CompromiseCards { get; set; }
        public List<GiftCard>? DoubleGiftCards { get; set; }

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
    }
}
