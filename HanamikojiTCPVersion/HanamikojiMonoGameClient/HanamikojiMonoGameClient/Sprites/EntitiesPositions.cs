using System.Collections.Generic;
using CommonResources.Game;
using HanamikojiMonoGameClient.Providers;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient.Sprites;

public static class EntitiesPositions
{
    public static readonly Vector2 TopDeckCardPosition = new Vector2(GameSettings.WINDOW_WIDTH - SpritesProvider.CardHeight, GameSettings.WINDOW_HEIGHT / 2.0f - SpritesProvider.CardWidth / 2.0f);

    public static readonly Vector2 FirstPlayerCardPosition = new Vector2(700, 800);

    public static readonly Vector2 FirstOpponentCardPosition = new Vector2(700, 0);

    public static readonly Vector2 PlayerSecretPosition = new Vector2(1300, 650);
    public static readonly Vector2 OpponentSecretPosition = new Vector2(50,150);

    public static readonly Vector2 PlayerEliminatedPosition = new Vector2(PlayerSecretPosition.X + (int)(1.5 * SpritesProvider.CardWidth), PlayerSecretPosition.Y);
    public static readonly Vector2 OpponentEliminatedPosition = new Vector2(OpponentSecretPosition.X + (int)(1.5 * SpritesProvider.CardWidth), OpponentSecretPosition.Y);

    public static readonly Vector2 FirstPlayerTradeOfferCardPosition = new Vector2(FirstOpponentCardPosition.X, (int)(FirstOpponentCardPosition.Y + 1.1 * SpritesProvider.CardHeight));

    private static readonly Vector2 _firstPlayerMovePosition = new Vector2(1300, 800);
    private static readonly Vector2 _firstOpponentMovePosition = new Vector2(50, 50);


    public static IDictionary<PlayerMoveTypeEnum, Vector2> PlayerMoveCardDefaultPositionDictionary =
        new Dictionary<PlayerMoveTypeEnum, Vector2>
        {
            { PlayerMoveTypeEnum.Compromise, _firstPlayerMovePosition},
            { PlayerMoveTypeEnum.DoubleGift, _firstPlayerMovePosition + new Vector2(75,0) },
            { PlayerMoveTypeEnum.Elimination, _firstPlayerMovePosition + new Vector2(150,0) },
            { PlayerMoveTypeEnum.Secret, _firstPlayerMovePosition + new Vector2(225,0) },
        };

    public static IDictionary<PlayerMoveTypeEnum, Vector2> OpponentMoveCardDefaultPositionDictionary =
        new Dictionary<PlayerMoveTypeEnum, Vector2>
        {
            { PlayerMoveTypeEnum.Compromise, _firstOpponentMovePosition },
            { PlayerMoveTypeEnum.DoubleGift, _firstOpponentMovePosition + new Vector2(75,0) },
            { PlayerMoveTypeEnum.Elimination, _firstOpponentMovePosition + new Vector2(150,0)},
            { PlayerMoveTypeEnum.Secret, _firstOpponentMovePosition + new Vector2(225,0)},
        };

    private static readonly Vector2 _firstGeishaPosition = new Vector2(100, GameSettings.WINDOW_HEIGHT / 2.0f - SpritesProvider.GeishaSize / 2.0f);
    private const int _gapBetweenGeisha = 5;


    public static IDictionary<GeishaType, Vector2> geishaPositions =
        new Dictionary<GeishaType, Vector2>
        {
            {GeishaType.Geisha2_A, _firstGeishaPosition },
            {GeishaType.Geisha2_B, _firstGeishaPosition + new Vector2(SpritesProvider.GeishaSize + _gapBetweenGeisha,0) },
            {GeishaType.Geisha2_C, _firstGeishaPosition + new Vector2(2*(SpritesProvider.GeishaSize + _gapBetweenGeisha) ,0)},
            {GeishaType.Geisha3_A, _firstGeishaPosition + new Vector2(3*(SpritesProvider.GeishaSize + _gapBetweenGeisha) ,0)},
            {GeishaType.Geisha3_B, _firstGeishaPosition + new Vector2(4*(SpritesProvider.GeishaSize + _gapBetweenGeisha) ,0)},
            {GeishaType.Geisha4_A, _firstGeishaPosition + new Vector2(5*(SpritesProvider.GeishaSize + _gapBetweenGeisha) ,0)},
            {GeishaType.Geisha5_A, _firstGeishaPosition + new Vector2(6*(SpritesProvider.GeishaSize + _gapBetweenGeisha),0)},
        };

    public static Vector2 GetPlayerGiftForGeishaFirstPosition(GeishaType geishaType)
    {
        var geishaIconPosition = geishaPositions[geishaType];

        return new Vector2(geishaIconPosition.X + SpritesProvider.GeishaSize / 2.0f - SpritesProvider.CardWidth / 2.0f, geishaIconPosition.Y + SpritesProvider.GeishaSize + 10);
    }

    public static Vector2 GetOpponentGiftForGeishaFirstPosition(GeishaType geishaType)
    {
        var geishaIconPosition = geishaPositions[geishaType];

        return new Vector2(geishaIconPosition.X + SpritesProvider.GeishaSize / 2.0f - SpritesProvider.CardWidth / 2.0f,  geishaIconPosition.Y - SpritesProvider.CardHeight - 10);
    }
}