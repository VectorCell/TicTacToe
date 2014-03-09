using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    class GameButton : GameObject
    {
        private string text;

        public GameButton(Game1 game, string text) : base(game)
        {
            this.text = text;
        }
    }
}
