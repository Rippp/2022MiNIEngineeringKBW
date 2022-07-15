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
    }
}
