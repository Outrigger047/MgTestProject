using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
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
        private const int maxMoveDist = 7;

        private readonly Vector2 diagInfoPos = new Vector2(5, 5);

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D playerRectangle;
        private Vector2 playerPos;
        private SpriteFont arial;
        private List<WorldShape> world = new List<WorldShape>();
        private List<WorldShape> worldRects = new List<WorldShape>();
        private List<WorldShape> worldEllipses = new List<WorldShape>();

        // Controller left stick position
        private float lsX;
        private float lsY;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;

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

            // Player initialization
            playerPos = new Vector2(20, 20);
            playerRectangle = new Texture2D(graphics.GraphicsDevice, 30, 30);
            Color[] playerColorData = new Color[30 * 30];
            for (int i = 0; i < playerColorData.Length; ++i)
            {
                playerColorData[i] = Color.White;
            }

            playerRectangle.SetData(playerColorData);

            // World geometry
            const int hsWallWidth = 15;
            Vector2 hsOrig = new Vector2(400, 300);
            Vector2 leftShrubCoord = new Vector2(430, 275);

            var geometries = new List<WorldShapeGeometry>
            {
#region Hard-coded geometry
                // House exterior walls
                new WorldShapeGeometry(hsOrig, 150, hsWallWidth, Shapes.Rectangle, 0, Color.DarkGray),
                new WorldShapeGeometry(new Vector2(hsOrig.X + 200, hsOrig.Y), 300, hsWallWidth, Shapes.Rectangle, 0, Color.DarkGray),
                new WorldShapeGeometry(new Vector2(hsOrig.X + 500, hsOrig.Y), hsWallWidth, 600, Shapes.Rectangle, 0, Color.DarkGray),
                new WorldShapeGeometry(new Vector2(hsOrig.X + 200 + hsWallWidth, hsOrig.Y + 600), 300,  hsWallWidth, Shapes.Rectangle, 0, Color.DarkGray),
                new WorldShapeGeometry(new Vector2(hsOrig.X, hsOrig.Y + hsWallWidth), hsWallWidth, 450, Shapes.Rectangle, 0, Color.DarkGray),
                new WorldShapeGeometry(new Vector2(hsOrig.X, hsOrig.Y + hsWallWidth + 450), 200, hsWallWidth, Shapes.Rectangle, 0, Color.DarkGray),
                new WorldShapeGeometry(new Vector2(hsOrig.X + 200, hsOrig.Y + hsWallWidth + 450), hsWallWidth, 150, Shapes.Rectangle, 0, Color.DarkGray),

                // House floor
                new WorldShapeGeometry(new Vector2(hsOrig.X + hsWallWidth, hsOrig.Y + hsWallWidth), 485, 450, Shapes.Rectangle, 0, Color.SaddleBrown, passable: true),
                new WorldShapeGeometry(new Vector2(hsOrig.X + 215, hsOrig.Y + 465), 285, 135, Shapes.Rectangle, 0, Color.SaddleBrown, passable: true),

                // Shrubs, front left
                new WorldShapeGeometry(leftShrubCoord, 30, 30, Shapes.Ellipse, 0, Color.DarkGreen),
                new WorldShapeGeometry(new Vector2(leftShrubCoord.X + 35, leftShrubCoord.Y), 30, 30, Shapes.Ellipse, 0, Color.DarkGreen),
                new WorldShapeGeometry(new Vector2(leftShrubCoord.X + 70, leftShrubCoord.Y), 30, 30, Shapes.Ellipse, 0, Color.DarkGreen),

                // Shrubs, front right
                new WorldShapeGeometry(new Vector2(leftShrubCoord.X + 220, leftShrubCoord.Y), 30, 30, Shapes.Ellipse, 0, Color.DarkGreen),
                new WorldShapeGeometry(new Vector2(leftShrubCoord.X + 255, leftShrubCoord.Y), 30, 30, Shapes.Ellipse, 0, Color.DarkGreen),
                new WorldShapeGeometry(new Vector2(leftShrubCoord.X + 290, leftShrubCoord.Y), 30, 30, Shapes.Ellipse, 0, Color.DarkGreen),

                // River
                new WorldShapeGeometry(new Vector2(1600, -20), 65, 450, Shapes.Rectangle, MathHelper.ToRadians(20), Color.DarkBlue),
                new WorldShapeGeometry(new Vector2(1450, 400), 65, 500, Shapes.Rectangle, MathHelper.ToRadians(5), Color.DarkBlue),
                new WorldShapeGeometry(new Vector2(1407, 898), 65, 700, Shapes.Rectangle, MathHelper.ToRadians(2), Color.DarkBlue)
#endregion
            };

            foreach (var geometry in geometries)
            {
                world.Add(new WorldShape(geometry, graphics.GraphicsDevice));
            }

            // Filtered collections of world geometry based on shape type
            worldRects = world.Where(x => x.Geometry.ShapeType == Shapes.Rectangle).ToList();
            worldEllipses = world.Where(x => x.Geometry.ShapeType == Shapes.Ellipse).ToList();

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

            // Draw world geometry - Rectangles
            foreach (var rect in worldRects)
            {
                var geo = rect.Geometry;
                spriteBatch.FillRectangle(
                    new Rectangle((int)geo.Position.X, (int)geo.Position.Y, geo.Width, geo.Height), 
                    geo.DrawColor, geo.RotAngle);
            }

            // Draw world geometry - Ellipses
            foreach (var ellipse in worldEllipses)
            {
                var geo = ellipse.Geometry;
                spriteBatch.DrawCircle(new Vector2(geo.Position.X, geo.Position.Y), geo.Height / 2, 40, geo.DrawColor);
            }

            // Character
            spriteBatch.Draw(playerRectangle, playerPos, Color.White);

            // Diagnostic info
            if (Ps4Input.Ps4IsConnected(0))
            {
                spriteBatch.DrawString(arial, $"{lsX}, {lsY}", diagInfoPos, Color.White);
            }
            
            spriteBatch.DrawString(arial, $"{playerPos.X}, {playerPos.Y}", new Vector2(playerPos.X, playerPos.Y + 30), Color.White);

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
                if (playerPos.Y <= bounds.Height)
                {
                    var distance = playerPos.Y;
                    playerPos.Y -= distance > actualMoveDist ? actualMoveDist : distance;
                }
                else
                {
                    playerPos.Y = 0;
                }
            }
            else if (kbState.IsKeyDown(Keys.S) || kbState.IsKeyDown(Keys.Down))
            {
                if (playerPos.Y + playerRectangle.Height <= bounds.Height)
                {
                    var distance = Math.Abs((playerPos.Y + playerRectangle.Height) - bounds.Height);
                    playerPos.Y += distance > actualMoveDist ? actualMoveDist : distance;
                }
                else
                {
                    playerPos.Y = bounds.Height - playerRectangle.Height - 1;
                }
            }

            // X-axis movement and collision detection
            if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left))
            {
                if (playerPos.X + playerRectangle.Width <= bounds.Width)
                {
                    var distance = playerPos.X;
                    playerPos.X -= distance > actualMoveDist ? actualMoveDist : distance;
                }
                else
                {
                    playerPos.X = 0;
                }
            }
            else if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right))
            {
                if (playerPos.X + playerRectangle.Width <= bounds.Width)
                {
                    var distance = Math.Abs((playerPos.X + playerRectangle.Width) - bounds.Width);
                    playerPos.X += distance > actualMoveDist ? actualMoveDist : distance;
                }
                else
                {
                    playerPos.X = bounds.Width - playerRectangle.Width - 1;
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

            var newPosX = playerPos.X + lsX * maxMoveDist;
            if (newPosX + playerRectangle.Width > bounds.Width)
            {
                playerPos.X = bounds.Width - playerRectangle.Width;
            }
            else if (newPosX < 0)
            {
                playerPos.X = 0;
            }
            else
            {
                playerPos.X = Convert.ToSingle(Math.Ceiling(Convert.ToDouble(newPosX)));
            }

            var newPosY = playerPos.Y + lsY * maxMoveDist;
            if (newPosY + playerRectangle.Height > bounds.Height)
            {
                playerPos.Y = bounds.Height - playerRectangle.Height;
            }
            else if (newPosY < 0)
            {
                playerPos.Y = 0;
            }
            else
            {
                playerPos.Y = Convert.ToSingle(Math.Ceiling(Convert.ToDouble(newPosY)));
            }
        }
    }
}
