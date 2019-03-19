using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MgTestProject
    {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
        {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private const int moveAmt = 9;
        private Texture2D rect;
        private Vector2 position;

        public Game1()
            {
            graphics = new GraphicsDeviceManager(this);
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
            position = new Vector2(20, 20);
            
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

            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var bounds = GraphicsDevice.Viewport.Bounds;
            var kbState = Keyboard.GetState();

            // Y-axis movement and collision detection
            if (kbState.IsKeyDown(Keys.W) || kbState.IsKeyDown(Keys.Up))
            {
                if (position.Y <= bounds.Height)
                {
                    var distance = position.Y;
                    position.Y -= distance > moveAmt ? moveAmt : distance;
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
                    position.Y += distance > moveAmt ? moveAmt : distance;
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
                    position.X -= distance > moveAmt ? moveAmt : distance;
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
                    position.X += distance > moveAmt ? moveAmt : distance;
                }
                else
                {
                    position.X = bounds.Width - rect.Width - 1;
                }
            }

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

            rect = new Texture2D(graphics.GraphicsDevice, 30, 30);

            Color[] data = new Color[30 * 30];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.White;
            }
            rect.SetData(data);

            Vector2 coor = position;
            spriteBatch.Draw(rect, coor, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
            }
        }
    }
