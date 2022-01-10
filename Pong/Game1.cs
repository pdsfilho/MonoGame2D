using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Series2D1
{
    #region Player Data
    public struct PlayerData
    {
        public Vector2 Position;
        public bool IsAlive;
        public Color Color;
        public float Angle;
        public float Power;
    }
    #endregion
    public class Game1 : Game
    {
        #region Properties
        //Textures and Graphics
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;
        private Texture2D _backgroundTexture;
        private Texture2D _foregroundTexture;

        //Player Textures
        private Texture2D _carriageTexture;
        private Texture2D _cannonTexture;
        private float _playerScaling;

        //Screen Info
        private int _screenWidth;
        private int _screenHeight;

        //Players
        private PlayerData[] _players;
        private int _numberOfPlayers = 4;

        private Color[] _playerColors = new Color[10]
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Purple,
            Color.Orange,
            Color.Indigo,
            Color.Yellow,
            Color.SaddleBrown,
            Color.Tomato,
            Color.Turquoise
        };
        #endregion
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 500;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Window.Title = "Monogame Tutorial";
            base.Initialize();
        }

        private void SetUpPlayers() 
        {
            _players = new PlayerData[_numberOfPlayers];
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                _players[i].IsAlive = true;
                _players[i].Color = _playerColors[i];
                _players[i].Angle = MathHelper.ToRadians(90);
                _players[i].Power = 100;
            }
            _players[0].Position = new Vector2(100, 193);
            _players[1].Position = new Vector2(200, 212);
            _players[2].Position = new Vector2(300, 361);
            _players[3].Position = new Vector2(400, 164);
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _device = _graphics.GraphicsDevice;
            _backgroundTexture = Content.Load<Texture2D>("background");
            _foregroundTexture = Content.Load<Texture2D>("foreground");
            _carriageTexture = Content.Load<Texture2D>("carriage");
            _cannonTexture = Content.Load<Texture2D>("cannon");

            _playerScaling = 40.0f / (float)_carriageTexture.Width;

            _screenWidth = _device.PresentationParameters.BackBufferWidth;
            _screenHeight = _device.PresentationParameters.BackBufferHeight;

            SetUpPlayers();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            DrawScenery();
            DrawPlayers();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawScenery()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, _screenWidth, _screenHeight);
            _spriteBatch.Draw(_backgroundTexture, screenRectangle, Color.White);
            _spriteBatch.Draw(_foregroundTexture, screenRectangle, Color.White);
        }

        private void DrawPlayers()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                if (_players[i].IsAlive)
                {
                    _spriteBatch.Draw(_carriageTexture, _players[i].Position, null,
                        _players[i].Color, 0, new Vector2(0, _carriageTexture.Height), _playerScaling, SpriteEffects.None, 0);
                }
            }
        }
    }
}
