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
        private SolidColorTexture White;

        private Rectangle viewportBounds;

        private Texture2D winner;

        private int size; // square
        private int offsetX;

        private int[,] gridState;
        private static readonly int EMPTY = 0;
        private static readonly int HAS_X = 1;
        private static readonly int HAS_O = 4;
        private bool gameWon;
        private bool showWin;

        private int turn;
        private static readonly int TURN_X = 0;
        private static readonly int TURN_O = 1;
        private static readonly int CATS = 2; // used for a tie
        private int player;

        public GameGrid(Game1 game) : base(game)
        {
            White = new SolidColorTexture(game, Color.White);

            gridState = new int[3,3];
            gameWon = false;
            showWin = false;

            turn = TURN_X;
            player = TURN_X;
        }

        public override void LoadContent()
        {
            winner = game.Content.Load<Texture2D>("winner");
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

        // transforms a vector from the 33^2 sector to real coordinates
        private Vector2 Transform(Vector2 v)
        {
            return new Vector2(offsetX + 33 * size / v.X, 33 * size / v.Y);
        }

        private int TransformX(int x)
        {
            return offsetX + x * size / 33;
        }

        private int TransformY(int y)
        {
            return Increment(y); // y * size / 33;
        }

        private int Increment(int inc)
        {
            return size * inc / 33;
        }

        private Rectangle TransformRect(int x, int y, int width, int height)
        {
            int tX = TransformX(x);
            int tY = TransformY(y);
            int incX = Increment(width);
            int incY = Increment(height);
            return new Rectangle(tX, tY, incX, incY);
        }

        private bool InBounds(Vector2 vector, Rectangle bounds)
        {
            return (vector.X >= bounds.X && vector.X <= bounds.X + bounds.Width)
                && (vector.Y >= bounds.Y && vector.Y <= bounds.Y + bounds.Height);
        }

        // returns the turn index of the winning player,
        // or -1 if there is no winner
        private int CheckWin()
        {
            Console.WriteLine();
            for (int xi = 0; xi < gridState.GetLength(0); xi++)
            {
                for (int yi = 0; yi < gridState.GetLength(1); yi++)
                {
                    char ch = '_';
                    if (gridState[xi, yi] == HAS_X)
                        ch = 'X';
                    if (gridState[xi, yi] == HAS_O)
                        ch = 'O';
                    Console.Write(ch + " ");
                }
                Console.WriteLine();
            }

            int sum;

            // checks verticals
            for (int xi = 0; xi < gridState.GetLength(0); xi++)
            {
                sum = 0;
                for (int yi = 0; yi < gridState.GetLength(1); yi++)
                {
                    sum += gridState[xi, yi];
                }
                if (sum == 3 * HAS_X)
                    return TURN_X;
                if (sum == 3 * HAS_O)
                    return TURN_O;
            }

            // checks horizontals
            for (int yi = 0; yi < gridState.GetLength(1); yi++)
            {
                sum = 0;
                for (int xi = 0; xi < gridState.GetLength(0); xi++)
                {
                    sum += gridState[xi, yi];
                }
                if (sum == 3 * HAS_X)
                    return TURN_X;
                if (sum == 3 * HAS_O)
                    return TURN_O;
            }

            // checks diagonals
            sum = 0;
            for (int xi = 0; xi < gridState.GetLength(0); xi++)
            {
                sum += gridState[xi, xi];
            }
            if (sum == 3 * HAS_X)
                return TURN_X;
            if (sum == 3 * HAS_O)
                return TURN_O;
            sum = 0;
            for (int xi = 0; xi < gridState.GetLength(0); xi++)
            {
                sum += gridState[xi, gridState.GetLength(1) - xi - 1];
            }
            if (sum == 3 * HAS_X)
                return TURN_X;
            if (sum == 3 * HAS_O)
                return TURN_O;

            // checks for tie
            // only true if there is an empty space
            sum = 0;
            for (int xi = 0; xi < gridState.GetLength(0); xi++)
            {
                for (int yi = 0; yi < gridState.GetLength(1); yi++)
                {
                    if (gridState[xi, yi] == EMPTY)
                        return -1;
                }
            }
            return CATS;
        }

        public override void Update()
        {
            base.Update();

            viewportBounds = game.GraphicsDevice.Viewport.Bounds;
            size = Math.Min(viewportBounds.Width, viewportBounds.Height);
            offsetX = viewportBounds.Width / 2 - size / 2;
        }

        public void Update(MouseState lastMouse, MouseState currentMouse)
        {
            this.Update();

            if (lastMouse.LeftButton == ButtonState.Released && currentMouse.LeftButton == ButtonState.Pressed)
            {
                if (gameWon)
                {
                    gameWon = false;
                    showWin = false;
                    for (int xi = 0; xi < 3; xi++)
                    {
                        for (int yi = 0; yi < 3; yi++)
                        {
                            gridState[xi, yi] = EMPTY;
                        }
                    }
                }
                else
                {
                    showWin = false;
                    for (int xi = 0; xi < 3; xi++)
                    {
                        for (int yi = 0; yi < 3; yi++)
                        {
                            int x = 2 + xi * 10;
                            int y = 2 + yi * 10;
                            if (InBounds(new Vector2(currentMouse.X, currentMouse.Y), TransformRect(x, y, 9, 9)))
                            {
                                if (gridState[xi, yi] == EMPTY)
                                {
                                    gridState[xi, yi] = (turn == TURN_X ? HAS_X : HAS_O);
                                    turn = (turn == TURN_X ? TURN_O : TURN_X);
                                }
                            }
                        }
                    }

                    int winner = CheckWin();
                    if (winner >= 0)
                    {
                        gameWon = true;
                        showWin = true;
                        Console.WriteLine("We have a winner!");
                    }
                }
            }

            // computer player's turn
            if (turn != player)
            {

            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // background
            // spriteBatch.Draw(new SolidColorTexture(this.game, Color.Wheat), new Rectangle(offsetX, 0, size, size), Color.Black);

            // lines
            Color lineColor = Color.Black;
            spriteBatch.Draw(White, TransformRect(11, 2, 1, 29), lineColor);
            spriteBatch.Draw(White, TransformRect(21, 2, 1, 29), lineColor);
            spriteBatch.Draw(White, TransformRect(2, 11, 29, 1), lineColor);
            spriteBatch.Draw(White, TransformRect(2, 21, 29, 1), lineColor);

            // spaces
            for (int xi = 0; xi < 3; xi++)
            {
                for (int yi = 0; yi < 3; yi++)
                {
                    int x = 2 + xi * 10;
                    int y = 2 + yi * 10;
                    Rectangle clickArea = TransformRect(x, y, 9, 9);
                    Rectangle drawArea = TransformRect(x + 1, y + 1, 7, 7);
                    // spriteBatch.Draw(White, clickArea, Color.DarkCyan);
                    if (gridState[xi,yi] == HAS_X)
                        spriteBatch.Draw(White, drawArea, Color.Blue);
                    else if (gridState[xi,yi] == HAS_O)
                        spriteBatch.Draw(White, drawArea, Color.Red);
                }
            }

            if (showWin)
            {
                spriteBatch.Draw(winner, GUIUtil.Center(winner.Bounds, viewportBounds), Color.White);
            }

            // spriteBatch.Draw(new SolidColorTexture(this.game, Color.Cyan), new Rectangle(500, 100, 100, 100), Color.White);
        }
    }
}
