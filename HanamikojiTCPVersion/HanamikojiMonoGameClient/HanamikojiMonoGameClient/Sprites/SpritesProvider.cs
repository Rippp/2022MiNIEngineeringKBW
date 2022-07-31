using System;
using CommonResources.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.Sprites
{
    public static class SpritesProvider
    {
        public const int CardWidth = 78;
        public const int CardHeight = 117;

        public const int GeishaSize = 175;

        private const string CardsAssetsName = "Cards";
        private static Texture2D _cardsTextures;

        public static void LoadTexture(Game game)
        {
            _cardsTextures = game.Content.Load<Texture2D>(CardsAssetsName);
        }

        public static Sprite GetGiftCardSprite(GeishaType geishaType) => geishaType switch
        {
            GeishaType.Geisha2_A => new (_cardsTextures, 7, 10, CardWidth, CardHeight),
            GeishaType.Geisha2_B => new(_cardsTextures, 95, 10, CardWidth, CardHeight),
            GeishaType.Geisha2_C => new(_cardsTextures, 180, 10, CardWidth, CardHeight),
            GeishaType.Geisha3_A => new(_cardsTextures, 265, 10, CardWidth, CardHeight),
            GeishaType.Geisha3_B => new(_cardsTextures, 349, 10, CardWidth, CardHeight),
            GeishaType.Geisha4_A => new(_cardsTextures, 433, 10, CardWidth, CardHeight),
            GeishaType.Geisha5_A => new(_cardsTextures, 518, 10, CardWidth, CardHeight),
            GeishaType.AnonymizedGeisha => new(_cardsTextures, 603, 10, CardWidth, CardHeight),
            _ => throw new ArgumentOutOfRangeException(nameof(geishaType), geishaType, null)
        };

        public static Sprite GetMoveCardSprite(PlayerMoveTypeEnum geishaType) => geishaType switch
        {
            PlayerMoveTypeEnum.Elimination => GetEliminationMoveSprite(),
            PlayerMoveTypeEnum.Secret => GetSecretMoveSprite(),
            PlayerMoveTypeEnum.DoubleGift => GetDoubleGiftMoveSprite(),
            PlayerMoveTypeEnum.Compromise => GetCompromiseMoveSprite(),
            _ => throw new ArgumentOutOfRangeException(nameof(geishaType), geishaType, null)
        };

        public static Sprite GetGeishaSprite(GeishaType geishaType) => geishaType switch
        {
            GeishaType.Geisha2_A => new(_cardsTextures, 4, 254, GeishaSize, GeishaSize),
            GeishaType.Geisha2_B => new(_cardsTextures, 184, 253, GeishaSize, GeishaSize),
            GeishaType.Geisha2_C => new(_cardsTextures, 361, 253, GeishaSize, GeishaSize),
            GeishaType.Geisha3_A => new(_cardsTextures, 542, 254, GeishaSize, GeishaSize),
            GeishaType.Geisha3_B => new(_cardsTextures, 728, 256, GeishaSize, GeishaSize),
            GeishaType.Geisha4_A => new(_cardsTextures, 911, 255, GeishaSize, GeishaSize),
            GeishaType.Geisha5_A => new(_cardsTextures, 1093, 254, GeishaSize, GeishaSize),
            _ => throw new ArgumentOutOfRangeException(nameof(geishaType), geishaType, null)
        };

        private static Sprite GetSecretMoveSprite() => new(_cardsTextures, 8, 172, 55, 53);
        private static Sprite GetEliminationMoveSprite() => new(_cardsTextures, 86, 172, 55, 53);
        private static Sprite GetDoubleGiftMoveSprite() => new(_cardsTextures, 156, 172, 55, 53);
        private static Sprite GetCompromiseMoveSprite() => new(_cardsTextures, 228, 173, 56, 53);
    }
}
