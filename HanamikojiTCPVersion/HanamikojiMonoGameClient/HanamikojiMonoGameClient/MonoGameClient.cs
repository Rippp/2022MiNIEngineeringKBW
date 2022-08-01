using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonResources.Game;
using HanamikojiClient;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Managers;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient
{
    public class MonoGameClient : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TcpGameClient _tcpGameClient;
        TableManager _tableManager;
        InputManager _inputManager;
        PointedCardAnimator _pointedCardAnimator;

        private Button _submitButton;

        private List<GameEntity> _gameEntities;
        private IDictionary<Guid, GiftCardEntity> _giftCardEntityDictionary = new Dictionary<Guid, GiftCardEntity>();

        private SpriteFont _messageFont;
        private string _message;

        public MonoGameClient(TcpGameClient tcpGameClient)
        {
            _tcpGameClient = tcpGameClient;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _gameEntities = new List<GameEntity>();



            Task.Run(() =>
            {
                tcpGameClient.ConnectToServer();
                tcpGameClient.Run();
            });
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            TargetElapsedTime = TimeSpan.FromSeconds(1f / (GameSettings.FPS));
            IsFixedTimeStep = true;

            base.Initialize();

            _graphics.PreferredBackBufferHeight = GameSettings.WINDOW_HEIGHT;
            _graphics.PreferredBackBufferWidth = GameSettings.WINDOW_WIDTH;

            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpritesProvider.LoadTexture(this);

            // TODO: can be moved to TableManager constructor
            var playerMoves = new List<MoveCardEntity>
            {
                new MoveCardEntity(PlayerMoveTypeEnum.Secret, true),
                new MoveCardEntity(PlayerMoveTypeEnum.Compromise, true),
                new MoveCardEntity(PlayerMoveTypeEnum.DoubleGift, true),
                new MoveCardEntity(PlayerMoveTypeEnum.Elimination, true),
            };

            var opponentMoves = new List<MoveCardEntity>
            {
                new MoveCardEntity(PlayerMoveTypeEnum.Secret, false),
                new MoveCardEntity(PlayerMoveTypeEnum.Compromise, false),
                new MoveCardEntity(PlayerMoveTypeEnum.DoubleGift, false),
                new MoveCardEntity(PlayerMoveTypeEnum.Elimination, false),
            };

            var geishaIcons = new List<GeishaEntity>
            {
                new GeishaEntity(GeishaType.Geisha2_A),
                new GeishaEntity(GeishaType.Geisha2_B),
                new GeishaEntity(GeishaType.Geisha2_C),
                new GeishaEntity(GeishaType.Geisha3_A),
                new GeishaEntity(GeishaType.Geisha3_B),
                new GeishaEntity(GeishaType.Geisha4_A),
                new GeishaEntity(GeishaType.Geisha5_A),
            };

            _submitButton = new Button();

            _tableManager = new TableManager(playerMoves, opponentMoves, _giftCardEntityDictionary);
            _inputManager = new InputManager(_gameEntities, _giftCardEntityDictionary, _tcpGameClient, _submitButton);
            _pointedCardAnimator = new PointedCardAnimator(_giftCardEntityDictionary);

            _gameEntities.AddRange(playerMoves);
            _gameEntities.AddRange(opponentMoves);
            _gameEntities.AddRange(geishaIcons);
            _gameEntities.Add(_submitButton);
            SortGameEntitiesByDrawOrder();

            _messageFont = Content.Load<SpriteFont>("messageFont");
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            var mouseState = Mouse.GetState();
            var gameData = _tcpGameClient.GetGameData();

            _tableManager.Update(gameData, gameTime);
            _inputManager.Update(gameData, mouseState);
            _pointedCardAnimator.Update(gameData, mouseState);

            _gameEntities.AddRange(_tableManager.GiveRecentlyAddedEntities());
            _gameEntities.ForEach(x => x.Update(gameTime));
            SortGameEntitiesByDrawOrder();

            _message = gameData.MessageToCurrentPlayer;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();

            // TODO: Add your drawing code here

            _gameEntities.ForEach(x => x.Draw(_spriteBatch, gameTime));

            _spriteBatch.DrawString(_messageFont, _message, new Vector2(0, 0), Color.Black);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void SortGameEntitiesByDrawOrder()
            => _gameEntities.Sort((x, y) => x.DrawOrder.CompareTo(y.DrawOrder));
    }
}
