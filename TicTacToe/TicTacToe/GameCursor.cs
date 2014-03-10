using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TicTacToe
{
    class GameCursor : GameObject
    {
        private static float DEFAULT_SCALE = 0.1f;

        private int numClicks = 0;

        private float scale = DEFAULT_SCALE;
        private int offsetX = -50; // location of focus of pointer in texture
        private int offsetY = -40;
        private int x = 0;
        private int y = 0;

        public GameCursor(Game1 game) : base(game)
        {
            this.texture = game.Content.Load<Texture2D>("cursor");
            this.drawRect = makeDrawRect();
        }

        private Rectangle makeDrawRect()
        {
            return new Rectangle(x + (int)(offsetX * scale), y + (int)(offsetY * scale), (int)(texture.Width * scale), (int)(texture.Height * scale));
        }

        public void Update(MouseState lastMouse, MouseState currentMouse)
        {
            x = currentMouse.X;
            y = currentMouse.Y;
            this.drawRect = makeDrawRect();

            if (lastMouse.LeftButton == ButtonState.Released && currentMouse.LeftButton == ButtonState.Pressed)
            {
                numClicks++;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!game.IsMouseVisible)
                base.Draw(spriteBatch);
        }
    }
}
