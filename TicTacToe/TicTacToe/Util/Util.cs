using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe.Util
{
    public class Util
    {
        /// <summary>
        /// Instance of Random for the sake of only using one source of
        /// pseudo-random numbers throughout TicTacToe.
        /// </summary>
        public static readonly Random RAND = new Random();
    }
}
