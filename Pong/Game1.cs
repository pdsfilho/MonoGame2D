﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
        private Texture2D _groundTexture;

        //Player Textures
        private Texture2D _carriageTexture;
        private Texture2D _cannonTexture;
        private float _playerScaling;

        //Projectiles
        private Texture2D _rocketTexture;
        private bool _rocketFlying = false;
        private Vector2 _rocketPosition;
        private Vector2 _rocketDirection;
        private float _rocketAngle;
        private float _rocketScaling = 0.1f;

        //Smoke
        private Texture2D _smokeTexture;
        private List<Vector2> _smokeList = new List<Vector2>();
        private Random _randomizer = new Random();

        //Screen Info
        private int _screenWidth;
        private int _screenHeight;

        //Players
        private PlayerData[] _players;
        private int _numberOfPlayers = 4;
        private int _currentPlayer = 0;

        //Texts
        private SpriteFont _font;

        //Terrain
        private int[] _terrainContour;

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
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Window.Title = "Monogame Cannon Battle";
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
                _players[i].Position = new Vector2();
                _players[i].Position.X = _screenWidth / (_numberOfPlayers + 1) * (i + 1);
                _players[i].Position.Y = _terrainContour[(int)_players[i].Position.X];
            }
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _device = _graphics.GraphicsDevice;
            _backgroundTexture = Content.Load<Texture2D>("background");
            _groundTexture = Content.Load<Texture2D>("ground");

            _carriageTexture = Content.Load<Texture2D>("carriage");
            _cannonTexture = Content.Load<Texture2D>("cannon");
            _rocketTexture = Content.Load<Texture2D>("rocket");
            _smokeTexture = Content.Load<Texture2D>("smoke");
            

            //Adding Text
            _font = Content.Load<SpriteFont>("myFont");

            _playerScaling = 40.0f / (float)_carriageTexture.Width;

            _screenWidth = _device.PresentationParameters.BackBufferWidth;
            _screenHeight = _device.PresentationParameters.BackBufferHeight;
            
           //Terrain
            GenerateTerrainContour();
            
            SetUpPlayers();
            FlattenTerrainBelowPlayers();
            CreateForeground();
        }
        private void GenerateTerrainContour()
        {
            _terrainContour = new int[_screenWidth];

            double rand1 = _randomizer.NextDouble() + 1;
            double rand2 = _randomizer.NextDouble() + 2;
            double rand3 = _randomizer.NextDouble() + 3;

            //Terrain's random wave
            float offset = _screenHeight / 2;
            float peakheight = 100;
            float flatness = 50;

            for (int x = 0; x < _screenWidth; x++)
            {
                double height = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakheight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakheight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                _terrainContour[x] = (int)height;
            }
        }
        //Store one color for each pixel on screen
        private void CreateForeground()
        {
            Color[] foregroundColors = new Color[_screenWidth * _screenHeight];
            Color[,] groundColors = TextureTo2DArray(_groundTexture);

            for (int x = 0; x < _screenWidth; x++)
            {
                for (int y = 0; y < _screenHeight; y++)
                {
                    if (y > _terrainContour[x])
                    {
                        foregroundColors[x + y * _screenWidth] = groundColors[x % _groundTexture.Width, y % _groundTexture.Height];
                    }
                    else
                    {
                        foregroundColors[x + y * _screenWidth] = Color.Transparent;
                    }
                }
            }
            _foregroundTexture = new Texture2D(_device, _screenWidth, _screenHeight, false, SurfaceFormat.Color);
            _foregroundTexture.SetData(foregroundColors);
        }
        private void FlattenTerrainBelowPlayers()
        {
            foreach (PlayerData player in _players)
            {
                if (player.IsAlive)
                {
                    for (int x = 0; x < 40; x++)
                    {
                        _terrainContour[(int)player.Position.X + x] = _terrainContour[(int)player.Position.X];
                    }
                }
            }
        }

        //Data extraction from a Texture2D
        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * texture.Width];
                }
            }

            return colors2D;

        }


        private void ProcessKeyboard()
        {
            KeyboardState keybState = Keyboard.GetState();
            if (keybState.IsKeyDown(Keys.A))
            {
                _players[_currentPlayer].Angle -= 0.01f;
            }
            if (keybState.IsKeyDown(Keys.D))
            {
                _players[_currentPlayer].Angle += 0.01f;
            }

            //Pi = 3.14 = 180º
            //PiOver2 = 90º
            //Angle not able to aim at the ground
            if (_players[_currentPlayer].Angle > MathHelper.PiOver2)
            {
                _players[_currentPlayer].Angle = MathHelper.PiOver2;
            }
            if (_players[_currentPlayer].Angle < -MathHelper.PiOver2)
            {
                _players[_currentPlayer].Angle = -MathHelper.PiOver2;
            }

            //Power of the cannon.
            if (keybState.IsKeyDown(Keys.S))
            {
                _players[_currentPlayer].Power -= 1;
            }
            if (keybState.IsKeyDown(Keys.W))
            {
                _players[_currentPlayer].Power += 1;
            }
            if (keybState.IsKeyDown(Keys.Down))
            {
                _players[_currentPlayer].Power -= 20;
            }
            if (keybState.IsKeyDown(Keys.Up))
            {
                _players[_currentPlayer].Power += 20;
            }

            if (_players[_currentPlayer].Power > 1000)
            {
                _players[_currentPlayer].Power = 1000;
            }
            if (_players[_currentPlayer].Power < 0)
            {
                _players[_currentPlayer].Power = 0;
            }

            //Projectile: Rocket
            if (keybState.IsKeyDown(Keys.Enter) || keybState.IsKeyDown(Keys.Space))
            {
                _rocketFlying = true;
                _rocketPosition = _players[_currentPlayer].Position;
                _rocketPosition.X += 20;
                _rocketPosition.Y -= 10;
                _rocketAngle = _players[_currentPlayer].Angle;

                //Calculates the projectile dropping arround the screen.
                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(_rocketAngle);
                _rocketDirection = Vector2.Transform(up, rotMatrix);
                _rocketDirection *= _players[_currentPlayer].Power / 50.0f;
            }
        }

        public void UpdateRocket()
        {
            if (_rocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);

                // X and Y are interchanged due to -Y being necessary to be upwards in Vector2
                _rocketAngle = (float)Math.Atan2(_rocketDirection.X, -_rocketDirection.Y);
                _rocketDirection += gravity / 10.0f;
                _rocketPosition += _rocketDirection;

                //Adds 5 particles of smoke
                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = _rocketPosition;
                    smokePos.X += _randomizer.Next(10) - 5;
                    smokePos.Y += _randomizer.Next(10) - 5;
                    _smokeList.Add(smokePos);
                   
                    //Clears smoke and rocket if rocket goes beyond screen's size on X and below ground on positive Y.
                    if (_rocketPosition.Y > 1000
                        || _rocketPosition.X > 1000 || _rocketPosition.X <- 1000)
                    {
                        _rocketFlying = false;
                        _smokeList.Clear();
                    }
                }
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            ProcessKeyboard();
            UpdateRocket();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            DrawScenery();
            DrawPlayers();
            DrawText();
            DrawRocket();
            DrawSmoke();
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
                    int xPos = (int)_players[i].Position.X;
                    int yPos = (int)_players[i].Position.Y;
                    Vector2 cannonOrigin = new Vector2(11, 50);

                    _spriteBatch.Draw(_carriageTexture, _players[i].Position, null,
                        _players[i].Color, 0, new Vector2(0, _carriageTexture.Height), _playerScaling, SpriteEffects.None, 0);

                    _spriteBatch.Draw(_cannonTexture, new Vector2(xPos + 20, yPos - 10), null,
                        _players[i].Color, _players[i].Angle, cannonOrigin, _playerScaling, SpriteEffects.None, 1);
                }
            }
        }
        private void DrawRocket()
        {
            if (_rocketFlying)
            {
                _spriteBatch.Draw(_rocketTexture, _rocketPosition, null, _players[_currentPlayer].Color,
                    _rocketAngle, new Vector2(42, 240), _rocketScaling, SpriteEffects.None, 1);
            }
        }

        private void DrawSmoke()
        {
            for (int i = 0; i < _smokeList.Count; i++)
            {
                _spriteBatch.Draw(_smokeTexture, _smokeList[i], null, Color.White,
                    0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);
            }
        }

        private void DrawText()
        {
            PlayerData player = _players[_currentPlayer];
            int currentAngle = (int)MathHelper.ToDegrees(player.Angle);
            _spriteBatch.DrawString(_font, "Cannon angle: " + currentAngle.ToString(), new Vector2(20, 20), player.Color);
            _spriteBatch.DrawString(_font, "Cannon power: " + player.Power.ToString(), new Vector2(20, 45), Color.White);
        }
    }
}
