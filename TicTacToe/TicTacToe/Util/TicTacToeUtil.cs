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
    public class TicTacToeUtil
    {
        // DEFINED CONSTANTS
        public static readonly int EMPTY = 0;
        public static readonly int PLAYER_X = 1;
        public static readonly int PLAYER_O = 4;
        public static readonly int TIE = 16;

        /// <summary>
        /// Returns the best move that a given player can make, given the current state of the game.
        /// Returns null if the board is full, or the game has already been won.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int[] GetBestMove(int[,] state, int player)
        {
            bool stillRoom = false;
            for (int x = 0; x < 3; x++)
                for (int y = 0; x < 3; y++)
                    stillRoom = stillRoom || state[x, y] == EMPTY;
            if (!stillRoom || IsWin(state, PLAYER_X) || IsWin(state, PLAYER_X))
                return null;

            int[] move;
            LinkedList<int[]> moves;

            // 1
            // if player can win, win
            move = CanWin(state, player);
            if (move != null)
                return move;

            // 2
            // if opponent can win, block
            move = CanWin(state, player == PLAYER_X ? PLAYER_O : PLAYER_X);
            if (move != null)
                return move;

            // 3
            // if player can fork, fork
            moves = GetFork(state, player);
            if (moves != null)
                return moves.First.Value;

            // 4
            // if opponent can fork, block
            moves = GetFork(state, player == PLAYER_X ? PLAYER_O : PLAYER_X);
            if (moves.Count >= 2)
            {
                // make opponent chase player
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        int[,] copy = CopyState(state);
                        copy[x, y] = player;
                        move = CanWin(copy, player);
                        if (move != null)
                            return move;
                    }
                }
            }
            else if (moves.Count == 1)
            {
                return moves.First.Value;
            }

            // 5
            // if center is available, go center
            if (state[1, 1] == EMPTY)
                return new int[] { 1, 1 };

            // 6
            // if corner is available, go corner
            if (state[0, 0] == EMPTY)
                return new int[] { 0, 0 };
            if (state[0, 2] == EMPTY)
                return new int[] { 0, 2 };
            if (state[2, 0] == EMPTY)
                return new int[] { 2, 0 };
            if (state[2, 2] == EMPTY)
                return new int[] { 2, 2 };

            // 7
            // go anywhere else (edge)
            if (state[0, 1] == EMPTY)
                return new int[] { 0, 1 };
            if (state[1, 0] == EMPTY)
                return new int[] { 1, 0 };
            if (state[1, 2] == EMPTY)
                return new int[] { 1, 2 };
            if (state[2, 1] == EMPTY)
                return new int[] { 2, 1 };

            // this condition should never be met, but it won't compile otherwise
            return null;
        }

        /// <summary>
        /// Returns a move that the player can make to win the game,
        /// or null if there is no such move.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int[] CanWin(int[,] state, int player)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (state[x, y] == EMPTY)
                    {
                        int[,] copy = CopyState(state);
                        copy[x, y] = player;
                        if (IsWin(copy, player))
                            return new int[] { x, y };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if the given player has won the game.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool IsWin(int[,] state, int player)
        {
            int[] pattern = new int[3];
            for (int k = 0; k < 3; k++)
                pattern[k] = player;
            return FindPattern(pattern, state).Count >= 1;
        }

        /// <summary>
        /// Returns a list of moves that the given player can make to set themself up for a fork.
        /// Returns null if there is no such move.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public static LinkedList<int[]> GetFork(int[,] state, int player)
        {
            LinkedList<int[]> moves = new LinkedList<int[]>();
            int[,] potential;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (state[x, y] == EMPTY)
                    {
                        potential = CopyState(state);
                        potential[x, y] = player;
                        if (IsFork(potential, player))
                            moves.AddLast(new int[] { x, y });
                    }
                }
            }
            return moves.Count > 0 ? moves : null;
        }

        public static bool IsFork(int[,] state, int player)
        {
            LinkedList<LinkedList<int[]>> fp1 = FindPattern(new int[] { player, player, EMPTY }, state);
            LinkedList<LinkedList<int[]>> fp2 = FindPattern(new int[] { player, EMPTY, player }, state);
            return (fp1.Count + fp2.Count) >= 2;
        }

        /// <summary>
        /// Returns a list of all instances of this patterns.
        /// The returned value is a list of sets of coordinates where this pattern has been found,
        /// or null if no instance of this pattern was found.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static LinkedList<LinkedList<int[]>> FindPattern(int[] pattern, int[,] state)
        {
            LinkedList<LinkedList<int[]>> list = new LinkedList<LinkedList<int[]>>();

            int[] reverse = Reverse(pattern);
            LinkedList<int[]> moves;
            bool matchP;
            bool matchR;

            // checks in x direction (vertically)
            for (int x = 0; x < 3; x++)
            {
                moves = new LinkedList<int[]>();
                matchP = matchR = true;
                for (int y = 0; y < 3; y++)
                {
                    matchP = matchP && state[x, y] == pattern[y];
                    matchR = matchR && state[x, y] == reverse[y];
                    moves.AddLast(new int[] { x, y });
                }
                if (matchP || matchR)
                    list.AddLast(moves);
            }

            // checks in y direction (horizontally)
            for (int y = 0; y < 3; y++)
            {
                moves = new LinkedList<int[]>();
                matchP = matchR = true;
                for (int x = 0; x < 3; x++)
                {
                    matchP = matchP && state[x, y] == pattern[x];
                    matchR = matchR && state[x, y] == reverse[x];
                    moves.AddLast(new int[] { x, y });
                }
                if (matchP || matchR)
                    list.AddLast(moves);
            }

            // checks diagonal from (0,0) to (2,2)
            moves = new LinkedList<int[]>();
            matchP = matchR = true;
            for (int n = 0; n < 3; n++)
            {
                matchP = matchP && state[n, n] == pattern[n];
                matchR = matchR && state[n, n] == reverse[n];
            }
            if (matchP || matchR)
                list.AddLast(moves);

            // checks diagonal from (0,2) to (2,0)
            moves = new LinkedList<int[]>();
            matchP = matchR = true;
            for (int n = 0; n < 3; n++)
            {
                matchP = matchP && state[n, 2-n] == pattern[n];
                matchR = matchR && state[n, 2-n] == reverse[n];
            }
            if (matchP || matchR)
                list.AddLast(moves);

            return list.Count >= 1 ? list : null;
        }

        public static int[] Reverse(int[] array)
        {
            int[] reverse = new int[array.Length];
            for (int k = 0; k < array.Length; k++)
                reverse[k] = array[(array.Length - 1) - k];
            return reverse;
        }

        public static int[,] CopyState(int[,] state)
        {
            int[,] copy = new int[state.GetLength(0), state.GetLength(1)];
            for (int x = 0; x < state.GetLength(0); x++)
                for (int y = 0; y < state.GetLength(1); y++)
                    copy[x, y] = state[x, y];
            return copy;
        }

        public static void Test()
        {
            Console.WriteLine("Testing...");
        }
    }
}
