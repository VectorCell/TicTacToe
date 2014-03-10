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

namespace TicTacToe.Util
{
    public static class GUIUtil
    {
        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end)
        {
            spriteBatch.Draw(texture, start, null, Color.White, (float)Math.Atan2(end.Y - start.Y, end.X - start.X),
                new Vector2(0.0f, (float)texture.Height / 2),
                new Vector2(Vector2.Distance(start, end), 1.0f),
                SpriteEffects.None, 0.0f);
        }
    }
}
