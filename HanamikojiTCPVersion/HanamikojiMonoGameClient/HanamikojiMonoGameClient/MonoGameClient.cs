using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CommonResources.Game;
using HanamikojiMonoGameClient.GameEntities;
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

        TcpGameClient _gameClient;
        TableManager _tableManager;

        private List<GameEntity> _gameEntities;

        public MonoGameClient(TcpGameClient gameClient)
        {
            _gameClient = gameClient;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Task.Run(() =>
            {
                gameClient.ConnectToServer();
                gameClient.Run();
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

            // TODO: use this.Content to load your game content here
            SpritesProvider.LoadTexture(this);

            _gameEntities = new List<GameEntity>();

            var playerMoves = new List<MoveCardEntity>
            {
                new MoveCardEntity(PlayerMoveTypeEnum.Secret),
                new MoveCardEntity(PlayerMoveTypeEnum.Compromise),
                new MoveCardEntity(PlayerMoveTypeEnum.DoubleGift),
                new MoveCardEntity(PlayerMoveTypeEnum.Elimination),
            };

            var opponentMoves = new List<MoveCardEntity>
            {
                new MoveCardEntity(PlayerMoveTypeEnum.Secret),
                new MoveCardEntity(PlayerMoveTypeEnum.Compromise),
                new MoveCardEntity(PlayerMoveTypeEnum.DoubleGift),
                new MoveCardEntity(PlayerMoveTypeEnum.Elimination),
            };

            _tableManager = new TableManager(playerMoves, opponentMoves);
            _gameEntities.AddRange(playerMoves);
            _gameEntities.AddRange(opponentMoves);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            var gameData = _gameClient.GetGameData();

            _tableManager.Update(gameData, gameTime);
            _gameEntities.AddRange(_tableManager.GiveRecentlyAddedEntities());

            _gameEntities.ForEach(x => x.Update(gameTime));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();

            // TODO: Add your drawing code here

            _gameEntities.ForEach(x => x.Draw(_spriteBatch, gameTime));

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
