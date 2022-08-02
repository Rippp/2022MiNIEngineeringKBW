using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonResources.Game;
using HanamikojiClient;
using HanamikojiMonoGameClient.GameEntities;
using HanamikojiMonoGameClient.Managers;
using HanamikojiMonoGameClient.Providers;
using HanamikojiMonoGameClient.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HanamikojiMonoGameClient
{
    public class MonoGameClient : Game, IGame
    {
        public Game Game => this;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ITcpGameClientProvider _tcpGameClientProvider;

        TableManager _tableManager;
        InputManager _inputManager;
        IPointedCardAnimator _pointedCardAnimator;
        IPointedEntityProvider _pointedEntityProvider;
        ClickedEntityProvider _clickedEntityProvider;

        private IEntitiesRepository _entitiesRepository;

        private Button _submitButton;

        

        private SpriteFont _messageFont;
        private string _message;

        public MonoGameClient(
            ITcpGameClientProvider tcpGameClientProvider, 
            TableManager tableManager,
            InputManager inputManager, 
            IEntitiesRepository entitiesRepository, 
            IPointedCardAnimator pointedCardAnimator, 
            IPointedEntityProvider pointedEntityProvider,
            ClickedEntityProvider clickedEntityProvider)
        {
            _tcpGameClientProvider = tcpGameClientProvider;
            _tableManager = tableManager;
            _inputManager = inputManager;
            _entitiesRepository = entitiesRepository;
            _pointedCardAnimator = pointedCardAnimator;
            _pointedEntityProvider = pointedEntityProvider;
            _clickedEntityProvider = clickedEntityProvider;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            var tcpGameClient = tcpGameClientProvider.GetTcpGameClient();

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
            
            _submitButton = new Button();
            _entitiesRepository.SetSubmitButtonEntity(_submitButton);

            _messageFont = Content.Load<SpriteFont>("messageFont");
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            var mouseState = Mouse.GetState();
            var gameData = _tcpGameClientProvider.GetTcpGameClient().GetGameData();

            _tableManager.Update(gameData, gameTime);
            _pointedEntityProvider.Update(gameData, mouseState);
            _pointedCardAnimator.Update(gameData, mouseState);
            _entitiesRepository.Update(gameTime);
            _inputManager.Update(gameData, mouseState);
            _clickedEntityProvider.Update(gameData, mouseState);

            _message = gameData.MessageToCurrentPlayer;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();

            var cardEntities = _entitiesRepository.GetAll(); 
            foreach (var cardEntity in cardEntities)
            {
                cardEntity.Draw(_spriteBatch, gameTime);
            }

            _spriteBatch.DrawString(_messageFont, _message, new Vector2(0, 0), Color.Black);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
