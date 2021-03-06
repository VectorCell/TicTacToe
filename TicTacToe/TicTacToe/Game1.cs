using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TicTacToe
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        // sprite drawing support
        LinkedList<GameObject> objects;

        KeyboardState currentKey;
        KeyboardState lastKey;
        MouseState currentMouse;
        MouseState lastMouse;

        private static readonly bool FULLSCREEN = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // preferred resolution
            if (FULLSCREEN)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
                this.IsMouseVisible = false;
                this.Window.AllowUserResizing = false;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
                graphics.IsFullScreen = false;
                this.IsMouseVisible = true;
                this.Window.AllowUserResizing = true;
                this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            }
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // this.IsMouseVisible = !this.IsMouseVisible;
        }

        public GraphicsDeviceManager GetGraphics()
        {
            return graphics;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            objects = new LinkedList<GameObject>();

            foreach (GameObject obj in objects)
            {
                obj.Initialize();
            }

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

            font = Content.Load<SpriteFont>("UI");

            GameBackground bg = new GameBackground(this);
            objects.AddLast(bg);

            GameGrid grid = new GameGrid(this);
            objects.AddLast(grid);

            GameButton closeButton = new GameButton(this, "Close", new Rectangle(
                this.GraphicsDevice.Viewport.Bounds.Width - 50,
                0, 50, 50));
            objects.AddLast(closeButton);

            // TODO: use this.Content to load your game content here
            GameCursor cursor = new GameCursor(this);
            objects.AddLast(cursor);

            foreach (GameObject obj in objects)
            {
                obj.LoadContent();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            lastKey = currentKey;
            currentKey = Keyboard.GetState();
            lastMouse = currentMouse;
            currentMouse = Mouse.GetState();

            if (lastMouse.LeftButton == ButtonState.Released && currentMouse.LeftButton == ButtonState.Pressed)
            {
                int x = currentMouse.X;
                int y = currentMouse.Y;
                if (x >= this.GraphicsDevice.Viewport.Bounds.Width - 50)
                    if (y < 50)
                        this.Exit();
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            foreach (GameObject obj in objects)
            {
                obj.Update();
                obj.Update(lastMouse, currentMouse);
            }
            // cursor.Update(lastMouse, currentMouse);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach (GameObject obj in objects)
            {
                obj.Draw(spriteBatch);
            }

            float frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
            spriteBatch.DrawString(font, 
                "FPS: " + Math.Round(frameRate, 1), 
                new Vector2(5, this.GraphicsDevice.Viewport.Bounds.Height - 42), 
                Color.White * 0.5f);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
