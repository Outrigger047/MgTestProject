using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PS4Mono;

namespace MgTestProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        /// <summary>
        /// Move amount per frame
        /// </summary>
        private const int maxMoveDist = 9;

        private readonly Vector2 diagInfoPos = new Vector2(5, 5);

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D rect;
        private Vector2 position;
        private SpriteFont arial;

        // Controller left stick position
        private float lsX;
        private float lsY;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Controller
            Ps4Input.Initialize(this);

            // Start position
            position = new Vector2(20, 20);

            rect = new Texture2D(graphics.GraphicsDevice, 30, 30);

            Color[] data = new Color[30 * 30];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.White;
            }

            rect.SetData(data);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            arial = Content.Load<SpriteFont>("arial");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();
            var gpState = GamePad.GetState(PlayerIndex.One);

            if (gpState.Buttons.Back == ButtonState.Pressed || kbState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            DoKeyboardMovement(kbState);
            DoGamepadMovement();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // Character
            spriteBatch.Draw(rect, position, Color.White);

            // Diagnostic info
            if (Ps4Input.Ps4IsConnected(0))
            {
                spriteBatch.DrawString(arial, $"{lsX}, {lsY}", diagInfoPos, Color.White);
            }
            
            spriteBatch.DrawString(arial, $"{position.X}, {position.Y}", new Vector2(position.X, position.Y + 30), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DoKeyboardMovement(KeyboardState kbState)
        {
            var bounds = GraphicsDevice.Viewport.Bounds;

            const double speedMultiplier = 0.5;
            int actualMoveDist = maxMoveDist;

            if (kbState.IsKeyDown(Keys.LeftShift) || kbState.IsKeyDown(Keys.RightShift))
                {
                actualMoveDist = Convert.ToInt32(Math.Ceiling(actualMoveDist * speedMultiplier));
                }

            // Y-axis movement and collision detection
            if (kbState.IsKeyDown(Keys.W) || kbState.IsKeyDown(Keys.Up))
            {
                if (position.Y <= bounds.Height)
                {
                    var distance = position.Y;
                    position.Y -= distance > actualMoveDist ? actualMoveDist : distance;
                }
                else
                {
                    position.Y = 0;
                }
            }
            else if (kbState.IsKeyDown(Keys.S) || kbState.IsKeyDown(Keys.Down))
            {
                if (position.Y + rect.Height <= bounds.Height)
                {
                    var distance = Math.Abs((position.Y + rect.Height) - bounds.Height);
                    position.Y += distance > actualMoveDist ? actualMoveDist : distance;
                }
                else
                {
                    position.Y = bounds.Height - rect.Height - 1;
                }
            }

            // X-axis movement and collision detection
            if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
            {
                if (position.X + rect.Width <= bounds.Width)
                {
                    var distance = position.X;
                    position.X -= distance > actualMoveDist ? actualMoveDist : distance;
                }
                else
                {
                    position.X = 0;
                }
            }
            else if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
            {
                if (position.X + rect.Width <= bounds.Width)
                {
                    var distance = Math.Abs((position.X + rect.Width) - bounds.Width);
                    position.X += distance > actualMoveDist ? actualMoveDist : distance;
                }
                else
                {
                    position.X = bounds.Width - rect.Width - 1;
                }
            }
        }

        private void DoGamepadMovement()
        {
            var bounds = GraphicsDevice.Viewport.Bounds;

            // Left stick snapshot
            lsX = Ps4Input.Ps4RawAxis(0, Axis.LeftX);
            lsY = Ps4Input.Ps4RawAxis(0, Axis.LeftY);

            // Left stick deadzone
            if (Math.Abs(lsX) < 0.09)
            {
                lsX = 0;
            }

            if (Math.Abs(lsY) < 0.09)
            {
                lsY = 0;
            }

            var newPosX = position.X + lsX * maxMoveDist;
            if (newPosX + rect.Width > bounds.Width)
            {
                position.X = bounds.Width - rect.Width;
            }
            else if (newPosX < 0)
            {
                position.X = 0;
            }
            else
            {
                position.X = newPosX;
            }

            var newPosY = position.Y + lsY * maxMoveDist;
            if (newPosY + rect.Height > bounds.Height)
            {
                position.Y = bounds.Height - rect.Height;
            }
            else if (newPosY < 0)
            {
                position.Y = 0;
            }
            else
            {
                position.Y = newPosY;
            }
        }
    }
}
