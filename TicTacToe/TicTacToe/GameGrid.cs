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

        SpriteFont font;

        private int size; // square
        private int offsetX;

        private static readonly int EMPTY = TicTacToeUtil.EMPTY;
        private static readonly int PLAYER_X = TicTacToeUtil.PLAYER_X;
        private static readonly int PLAYER_O = TicTacToeUtil.PLAYER_O;
        private static readonly int TIE = TicTacToeUtil.TIE;

        private int[,] gridState;

        private int turn;
        private int player;
        private int starting;
        private int winner;

        private bool computerPlayer;

        public GameGrid(Game1 game) : base(game)
        {
            White = new SolidColorTexture(game, Color.White);

            gridState = new int[3, 3];

            player = TicTacToeUtil.PLAYER_X;
            starting = TicTacToeUtil.PLAYER_X;
            turn = starting;
            winner = 0;
            computerPlayer = true;
        }

        public GameGrid(Game1 game, bool computerPlayer) : base(game)
        {
            White = new SolidColorTexture(game, Color.White);

            gridState = new int[3, 3];

            player = TicTacToeUtil.PLAYER_X;
            starting = TicTacToeUtil.PLAYER_X;
            turn = starting;
            winner = EMPTY;
            this.computerPlayer = computerPlayer;
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("UI");
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

        public override void Update()
        {
            base.Update();

            viewportBounds = game.GraphicsDevice.Viewport.Bounds;
            size = Math.Min(viewportBounds.Width, viewportBounds.Height);
            offsetX = viewportBounds.Width / 2 - size / 2;
        }

        public override void Update(MouseState lastMouse, MouseState currentMouse)
        {
            // player's turn
            if (lastMouse.LeftButton == ButtonState.Released && currentMouse.LeftButton == ButtonState.Pressed)
            {
                if (winner != EMPTY) // last game ended
                {
                    // reset the game and continue
                    for (int n = 0; n < 9; n++)
                        gridState[n / 3, n % 3] = EMPTY;
                    winner = EMPTY;
                    starting = TicTacToeUtil.GetOtherPlayer(starting);
                    turn = starting;
                    return;
                }

                bool validClick = false;
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
                                gridState[xi, yi] = turn;
                                validClick = true;
                            }
                        }
                    }
                }
                if (!validClick)
                    return;

                // go to next player
                turn = TicTacToeUtil.GetOtherPlayer(turn);
            }

            if (computerPlayer && turn == TicTacToeUtil.GetOtherPlayer(player) && !TicTacToeUtil.IsGameOver(gridState))
            {
                Console.WriteLine("Attemping to process computer's turn (" + TicTacToeUtil.GetPlayerName(turn) + ")");
                int[] move = TicTacToeUtil.GetBestMove(gridState, turn);
                if (move != null)
                {
                    gridState[move[0], move[1]] = turn;
                    Console.WriteLine("Computer decided that the best move is at {" + move[0] + "," + move[1] + "}");
                    turn = TicTacToeUtil.GetOtherPlayer(turn);
                }
                else
                {
                    Console.WriteLine("Computer has not made a move");
                }
            }

            // check to see if the game is over
            if (winner == EMPTY && TicTacToeUtil.IsGameOver(gridState))
            {
                winner = TicTacToeUtil.GetWinner(gridState);
                if (winner != EMPTY)
                {
                    if (winner == player)
                        Console.WriteLine("the player has won!");
                    else if (winner == TicTacToeUtil.GetOtherPlayer(turn))
                        Console.WriteLine("the player has lost");
                    else if (winner == TIE)
                        Console.WriteLine("the game was a tie");
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // lines
            Color lineColor = new Color(48, 48, 48); // Color.White * 0.25f;
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
                    if (gridState[xi,yi] == PLAYER_X)
                        spriteBatch.Draw(White, drawArea, Color.Blue * 0.5f);
                    else if (gridState[xi,yi] == PLAYER_O)
                        spriteBatch.Draw(White, drawArea, Color.Red * 0.5f);
                }
            }

            if (winner != EMPTY)
            {
                if (winner != TIE)
                    spriteBatch.DrawString(font,
                        winner == PLAYER_X ? "Blue Wins" : "Red Wins",
                        new Vector2(10, 10),
                        winner == PLAYER_X ? Color.Blue : Color.Red);
                else
                    spriteBatch.DrawString(font,
                        "Tie!",
                        new Vector2(10, 10),
                        Color.White);
            }
            else if (!computerPlayer)
            {
                spriteBatch.DrawString(font,
                    turn == PLAYER_X ? "Blue's Turn" : "Red's Turn",
                    new Vector2(10, 10),
                    turn == PLAYER_X ? Color.Blue : Color.Red);
            }

            // spriteBatch.Draw(new SolidColorTexture(this.game, Color.Cyan), new Rectangle(500, 100, 100, 100), Color.White);

            
        }
    }
}
