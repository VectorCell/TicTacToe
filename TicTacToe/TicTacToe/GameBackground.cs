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

using TicTacToe.Util;

namespace TicTacToe
{
    class GameBackground : GameObject
    {
        static Random r;

        static SolidColorTexture White;

        Vector4[] points;
        float speed;
        float baseSpeed;

        public GameBackground(Game1 game) : base(game)
        {
            r = new Random();
            points = new Vector4[1000];
            speed = 0.005f;
            baseSpeed = speed;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            White = new SolidColorTexture(game, Color.White);

            for (int k = 0; k < points.Length; k++)
            {
                points[k] = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            }

            texture = game.Content.Load<Texture2D>("starfield1");
            color = new Color(0, 0, 0);
        }

        public override void Update()
        {
            base.Update();
            drawRect = game.GraphicsDevice.Viewport.Bounds;
        }

        public override void Update(MouseState lastMouse, MouseState currentMouse)
        {
            int x = currentMouse.X;
            int y = currentMouse.Y;
            float percentX = (float)x / drawRect.Width;
            float percentY = (float)y / drawRect.Height;

            if (currentMouse.RightButton == ButtonState.Pressed)
            {
                if (speed < 0.05f)
                    speed *= 1.01f;
            }
            else if (speed > baseSpeed)
            {
                speed *= 0.95f;
            }
            else
            {
                speed = baseSpeed;
            }

            for (int k = 0; k < points.Length; k++)
            {
                Vector4 v = points[k];
                if (v.X > 0 && v.X < 1 && v.Y > 0 && v.Y < 1)
                {
                    v.X -= percentX;
                    v.X *= 1 + speed * v.Z;
                    v.X += percentX;
                    v.Y -= percentY;
                    v.Y *= 1 + speed * v.Z;
                    v.Y += percentY;
                    v.Z += speed;
                    v.W = 1.0f;
                    points[k] = v;
                }
                else
                {
                    points[k] = new Vector4((float)r.NextDouble(), (float)r.NextDouble(), (float)(r.NextDouble() / 32), (float)r.NextDouble());
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int k = 0; k < points.Length; k++)
            {
                Vector4 v = points[k];
                int value = r.Next();
                spriteBatch.Draw(White,
                    new Rectangle(
                        (int)(v.X * game.GraphicsDevice.Viewport.Bounds.Width),
                        (int)(v.Y * game.GraphicsDevice.Viewport.Bounds.Height),
                        (int)v.W, (int)v.W),
                    new Color((int)(255 * v.Z), (int)(255 * v.Z), (int)(255 * v.Z)));
            }
        }
    }
}
