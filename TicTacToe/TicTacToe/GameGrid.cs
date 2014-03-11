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

        private int[,] gridState;
        private static readonly int EMPTY = 0;
        private static readonly int HAS_X = 1;
        private static readonly int HAS_O = 4;
        private bool gameWon;
        private bool showWin;

        private int turn;
        private static readonly int PLAYER_X = 0;
        private static readonly int PLAYER_O = 1;
        private static readonly int TIE = 2;
        private int player;
        private int winner;

        private bool computerPlayer;

        public GameGrid(Game1 game) : base(game)
        {
            White = new SolidColorTexture(game, Color.White);

            gridState = new int[3,3];
            gameWon = false;
            showWin = false;

            turn = PLAYER_X;
            player = PLAYER_X;
            winner = -1;
            computerPlayer = true;
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

        /// <summary>
        /// Finds the given pattern in a line (either horizontally, vertically, or diagonally)
        /// in the grid (forwards or backwards).
        /// In the context of the GameGrid class, it's finding a pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        private int[,,] FindPattern(int[] pattern, int[,] grid)
        {
            int[] reverse = new int[pattern.Length];
            for (int k = 0; k < pattern.Length / 2; k++)
                reverse[k] = pattern[(pattern.Length - 1) - k];

            bool matchP = true;
            bool matchR = true;

            // list buffer of coordinates
            LinkedList<int[]> hold = new LinkedList<int[]>();

            // gets horizontal matches
            for (int x = 0; x < 3; x++)
            {
                matchP = true;
                matchR = true;
                for (int y = 0; y < 3; y++)
                {
                    matchP = matchP && (grid[x,y] == pattern[y]);
                    matchR = matchR && (grid[x,y] == reverse[y]);
                    hold.AddLast(new int[] { x, y });
                }
                if (!matchP && !matchR)
                    for (int k = 0; k < 3; k++)
                        hold.RemoveLast();
            }

            // gets vertical matches
            for (int y = 0; y < 3; y++)
            {
                matchP = true;
                matchR = true;
                for (int x = 0; x < 3; x++)
                {
                    matchP = matchP && (grid[x,y] == pattern[y]);
                    matchR = matchR && (grid[x,y] == reverse[y]);
                    hold.AddLast(new int[] { x, y });
                }
                if (!matchP && !matchR)
                    for (int k = 0; k < 3; k++)
                        hold.RemoveLast();
            }

            // gets diagonal matches
            matchP = true;
            matchR = true;
            for (int n = 0; n < 3; n++)
            {
                matchP = matchP && (grid[n, n] == pattern[n]);
                matchR = matchR && (grid[n, n] == reverse[n]);
                hold.AddLast(new int[] { n, n });
            }
            if (!matchP && !matchR)
                for (int k = 0; k < 3; k++)
                    hold.RemoveLast();

            // gets diagonal matches
            matchP = true;
            matchR = true;
            for (int n = 0; n < 3; n++)
            {
                matchP = matchP && (grid[n, 2-n] == pattern[n]);
                matchR = matchR && (grid[n, 2-n] == reverse[n]);
                hold.AddLast(new int[] { n, 2-n });
            }
            if (!matchP && !matchR)
                for (int k = 0; k < 3; k++)
                    hold.RemoveLast();

            int length = hold.Count;
            int numMatches = length / 6;
            int[, ,] matches = new int[numMatches, 3, 2];
            for (int match = 0; match < numMatches; match++)
            {
                for (int coord = 0; coord < 3; coord++)
                {
                    int[] place = hold.First();
                    hold.RemoveFirst();
                    for (int n = 0; n < place.Length; n++)
                        matches[match, coord, n] = place[n];
                }
            }
            return matches;
        }

        // returns the turn index of the winning player,
        // or -1 if there is no winner
        private int CheckWin()
        {
            return CheckWin(gridState);
        }

        // returns the turn index of the winning player,
        // or -1 if there is no winner
        private static int CheckWin(int[,] gridState)
        {
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
                    return PLAYER_X;
                if (sum == 3 * HAS_O)
                    return PLAYER_O;
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
                    return PLAYER_X;
                if (sum == 3 * HAS_O)
                    return PLAYER_O;
            }

            // checks diagonals
            sum = 0;
            for (int xi = 0; xi < gridState.GetLength(0); xi++)
            {
                sum += gridState[xi, xi];
            }
            if (sum == 3 * HAS_X)
                return PLAYER_X;
            if (sum == 3 * HAS_O)
                return PLAYER_O;
            sum = 0;
            for (int xi = 0; xi < gridState.GetLength(0); xi++)
            {
                sum += gridState[xi, gridState.GetLength(1) - xi - 1];
            }
            if (sum == 3 * HAS_X)
                return PLAYER_X;
            if (sum == 3 * HAS_O)
                return PLAYER_O;

            // checks for tie
            // only true if there is an empty space
            return HasMovesLeft(gridState) ? -1 : TIE;
        }

        // returns the move if the given player can make a single move to set up a fork
        // returns null if no such move exists
        private static int[] GetFork(int player, int[,] state)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (state[x,y] == EMPTY)
                    {
                        int[,] changed = CopyState(state);
                        changed[x, y] = (player == PLAYER_X ? HAS_X : HAS_O);
                        if (HasFork(player, changed))
                            return new int[] { x, y };
                    }
                }
            }
            return null;
        }

        // returns true if the given player has a fork
        private static bool HasFork(int player, int[,] state)
        {
            int has = (player == PLAYER_X ? HAS_X : HAS_O);
            int[] pattern1 = { has, has, EMPTY };
            int[] pattern2 = { has, EMPTY, has };
            int[] pattern3 = { EMPTY, has, has };

            return false;
        }

        private static bool HasMovesLeft(int[,] state)
        {
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    if (state[x, y] == EMPTY)
                        return true;
            return false;
        }

        private static int[,] CopyState(int[,] state)
        {
            int[,] copy = new int[state.GetLength(0), state.GetLength(1)];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    copy[x, y] = state[x, y];
            return copy;
        }

        /// <summary>
        /// Returns the best move for the current player.
        /// </summary>
        /// <returns></returns>
        private int[] BestMove()
        {
            // 1
            // checks for available win for the current player

            // 2
            // checks for available win for the other player

            // 3
            // checks for opportunity for current player to fork

            return null;
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
                    turn = player;
                }
                else
                {
                    showWin = false;
                    winner = -1;
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
                                    gridState[xi, yi] = (turn == PLAYER_X ? HAS_X : HAS_O);
                                    turn = (turn == PLAYER_X ? PLAYER_O : PLAYER_X);
                                }
                            }
                        }
                    }

                    winner = CheckWin();
                    if (winner >= 0)
                    {
                        gameWon = true;
                        showWin = true;
                        Console.WriteLine("We have a winner: " + winner);
                    }
                }
            }

            // computer player's turn
            if (computerPlayer && !gameWon && turn != player)
            {
                Console.WriteLine("computer's turn");
                int[] move = BestMove();
                bool moveMade = false;

                if (move != null)
                {
                    if (move.Length == 2 && move[0] >= 0 && move[1] >= 0 && gridState[move[0], move[1]] == EMPTY)
                    {
                        gridState[move[0], move[1]] = (turn == PLAYER_X ? HAS_X : HAS_O);
                        moveMade = true;
                    }
                    Console.WriteLine("Best move for player " + turn + ": " + move[0] + "," + move[1]);
                }

                if (!moveMade && HasMovesLeft(gridState))
                    Console.WriteLine("ERROR: NO MOVE MADE BY COMPUTER");
                else
                    turn = player;

                winner = CheckWin();
                if (winner >= 0)
                {
                    gameWon = true;
                    showWin = true;
                    Console.WriteLine("We have a winner: " + winner);
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
                    if (gridState[xi,yi] == HAS_X)
                        spriteBatch.Draw(White, drawArea, Color.Blue * 0.5f);
                    else if (gridState[xi,yi] == HAS_O)
                        spriteBatch.Draw(White, drawArea, Color.Red * 0.5f);
                }
            }

            if (showWin)
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
            else
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
