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

        Vector3[] points;
        float speed;

        public GameBackground(Game1 game) : base(game)
        {
            r = new Random();
            points = new Vector3[5000];
            speed = 0.01f;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            White = new SolidColorTexture(game, Color.White);

            for (int k = 0; k < points.Length; k++)
            {
                points[k] = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            }

            texture = game.Content.Load<Texture2D>("starfield1");
            color = new Color(0, 0, 0);
        }

        public override void Update()
        {
            base.Update();
            drawRect = game.GraphicsDevice.Viewport.Bounds;

            for (int k = 0; k < points.Length; k++)
            {
                Vector3 v = points[k];
                if (v.X > 0 && v.X < 1 && v.Y > 0 && v.Y < 1)
                {
                    v.X -= 0.5f;
                    v.X *= 1 + speed * v.Z;
                    v.X += 0.5f;
                    v.Y -= 0.5f;
                    v.Y *= 1 + speed * v.Z;
                    v.Y += 0.5f;
                    v.Z += speed;
                    points[k] = v;
                }
                else
                {
                    points[k] = new Vector3((float)r.NextDouble(), (float)r.NextDouble(), (float)(r.NextDouble() / 32));
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int k = 0; k < points.Length; k++)
            {
                Vector3 v = points[k];
                int value = r.Next();
                spriteBatch.Draw(White,
                    new Rectangle(
                        (int)(v.X * game.GraphicsDevice.Viewport.Bounds.Width),
                        (int)(v.Y * game.GraphicsDevice.Viewport.Bounds.Height),
                        1, 1),
                    new Color((int)(255 * v.Z), (int)(255 * v.Z), (int)(255 * v.Z)));
            }
        }
    }
}
