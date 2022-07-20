namespace CommonResources.Game.Constants
{
    public static class GiftCardConstants
    {
        public static List<GiftCard> GetAllCards()
        {
            return new()
            {
                new GiftCard(GeishaType.Geisha2_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha2_A, Guid.NewGuid()),

                new GiftCard(GeishaType.Geisha2_B, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha2_B, Guid.NewGuid()),

                new GiftCard(GeishaType.Geisha2_C, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha2_C, Guid.NewGuid()),

                new GiftCard(GeishaType.Geisha3_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha3_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha3_A, Guid.NewGuid()),

                new GiftCard(GeishaType.Geisha3_B, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha3_B, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha3_B, Guid.NewGuid()),

                new GiftCard(GeishaType.Geisha4_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha4_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha4_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha4_A, Guid.NewGuid()),

                new GiftCard(GeishaType.Geisha5_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha5_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha5_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha5_A, Guid.NewGuid()),
                new GiftCard(GeishaType.Geisha5_A, Guid.NewGuid())
            };
        }
    }
}