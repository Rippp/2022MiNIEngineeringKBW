using System;
using CommonResources.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HanamikojiMonoGameClient.Sprites
{
    public static class SpritesProvider
    {
        private const string CardsAssetsName = "Cards";
        private const int CardWidth = 155;
        private const int CardHeight = 233;
        private static Texture2D _cardsTextures;

        public static void LoadTexture(Game game)
        {
            _cardsTextures = game.Content.Load<Texture2D>(CardsAssetsName);
        }

        public static Sprite GetGiftCardSprite(GeishaType geishaType) => geishaType switch
        {
            GeishaType.Geisha2_A => YellowCardSprite(),
            GeishaType.Geisha2_B => RedCard(),
            GeishaType.Geisha2_C => YellowCardSprite(),
            GeishaType.Geisha3_A => YellowCardSprite(),
            GeishaType.Geisha3_B => YellowCardSprite(),
            GeishaType.Geisha4_A => YellowCardSprite(),
            GeishaType.Geisha5_A => YellowCardSprite(),
            GeishaType.AnonimizedGeisha => AnonimizedCard(),
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

        private static Sprite GetSecretMoveSprite() => new(_cardsTextures, 16, 345, 111, 107);
        private static Sprite GetEliminationMoveSprite() => new(_cardsTextures, 172, 345, 110, 107);
        private static Sprite GetDoubleGiftMoveSprite() => new(_cardsTextures, 312, 345, 106, 107);
        private static Sprite GetCompromiseMoveSprite() => new(_cardsTextures, 457, 345, 111, 108);
        private static Sprite YellowCardSprite() => new(_cardsTextures, 16, 20, CardWidth, CardHeight);
        private static Sprite RedCard() => new(_cardsTextures, 191, 21, CardWidth, CardHeight);
        private static Sprite AnonimizedCard() => new(_cardsTextures, 375, 21, CardWidth, CardHeight);
    }
}
