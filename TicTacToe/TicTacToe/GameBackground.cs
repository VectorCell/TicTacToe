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
    struct Point
    {
        private float x, y;
        private float speed;
        private float prevx, prevy;

        public Point(float x, float y, float speed)
        {
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.prevx = 0.0f;
            this.prevy = 0.0f;
        }

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                prevx = x;
                x = value;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                prevy = y;
                y = value;
            }
        }

        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public float PrevX
        {
            get
            {
                return prevx;
            }
        }

        public float PrevY
        {
            get
            {
                return prevy;
            }
        }
    }

    public class GameBackground : GameObject
    {
        private static Random r = Util.Util.RAND;

        private static SolidColorTexture White;
        private static SolidColorTexture[] colors;

        private static readonly int NUM_POINTS = 1500;

        Point[] points;
        float speed;
        float baseSpeed;

        public GameBackground(Game1 game) : base(game)
        {
            points = new Point[NUM_POINTS];
            colors = new SolidColorTexture[256];
            speed = 0.01f;
            baseSpeed = speed;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            White = new SolidColorTexture(game, Color.White);
            for (int k = 0; k < colors.Length; k++)
            {
                colors[k] = new SolidColorTexture(game, new Color(k, k, k));
            }

            for (int k = 0; k < NUM_POINTS; k++)
            {
                points[k] = new Point((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            }

            texture = new SolidColorTexture(game, Color.Black);
            color = Color.White;
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
                if (speed < (baseSpeed * 10))
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
                Point v = points[k];
                if (v.X > 0 && v.X < 1 && v.Y > 0 && v.Y < 1)
                {
                    v.X = (v.X - percentX) * (1 + speed * v.Speed) + percentX;
                    v.Y = (v.Y - percentY) * (1 + speed * v.Speed) + percentY;
                    v.Speed += speed;
                    points[k] = v;
                }
                else
                {
                    points[k] = new Point((float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble() / 32);
                }
            }
        }

        private int ScaleX(float percent)
        {
            return (int)(percent * game.GraphicsDevice.Viewport.Bounds.Width);
        }

        private int ScaleY(float percent)
        {
            return (int)(percent * game.GraphicsDevice.Viewport.Bounds.Height);
        }

        private float Displacement(Point p)
        {
            return (float)Math.Sqrt((double)Square(p.X - p.PrevX) + (double)Square(p.Y - p.PrevY));
        }

        private float Square(float f)
        {
            return f * f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // spriteBatch.DrawLine(this.game, Color.White, new Vector2(50, 50), new Vector2(150, 150));

            for (int k = 0; k < points.Length; k++)
            {
                Point v = points[k];
                int value = r.Next();
                if (v.PrevX == 0.0f || Displacement(v) < 0.01f)
                {
                    spriteBatch.Draw(colors[Math.Min(255, (int)(v.Speed * 255))],
                        new Rectangle(
                            ScaleX(v.X), ScaleY(v.Y),
                            1, 1),
                        Color.White);
                } 
                else
                {
                    spriteBatch.DrawLine(colors[Math.Min(255, (int)(Square(v.Speed) * 64))],
                        new Vector2(ScaleX(v.X), ScaleY(v.Y)), 
                        new Vector2(ScaleX(v.PrevX), ScaleY(v.PrevY)));
                }
            }

            if (speed > baseSpeed * 2)
            {
                int n = (int)(speed * 500);
                Color c = new Color(0, 0, n, n);
                spriteBatch.Draw(new SolidColorTexture(this.game, c), drawRect, Color.Blue);
            }

        }
    }
}
