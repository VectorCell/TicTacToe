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
    class GameGrid : GameObject
    {
        Rectangle viewportBounds;

        private int size; // square

        public GameGrid(Game1 game) : base(game)
        {
            
        }

        private void drawLine(int x1, int y1, int x2, int y2, Color color)
        {
            drawLine(x1, y1, x2, y2, color, color);
        }

        private void drawLine(int x1, int y1, int x2, int y2, Color color1, Color color2)
        {
            GraphicsDeviceManager graphics = game.GetGraphics();
            BasicEffect basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                         // near, far plane

            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0].Position = new Vector3(x1, y1, 0);
            vertices[0].Color = color1;
            vertices[1].Position = new Vector3(x2, y2, 0);
            vertices[1].Color = color2;

            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GetGraphics().GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }

        // not finished, doesn't draw thick line
        private void DrawThickLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
        {
            Texture2D texture = new SolidColorTexture(game, color);
            spriteBatch.DrawLine(texture, new Vector2(20, 20), new Vector2(120, 120));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            viewportBounds = game.GraphicsDevice.Viewport.Bounds;
            size = Math.Min(viewportBounds.Width, viewportBounds.Height);

            // background
            spriteBatch.Draw(new SolidColorTexture(this.game, Color.Wheat), new Rectangle(viewportBounds.Width / 2 - size / 2, 0, size, size), Color.White);

            // lines
            spriteBatch.

            spriteBatch.Draw(new SolidColorTexture(this.game, Color.Cyan), new Rectangle(500, 100, 100, 100), Color.White);
        }
    }
}
