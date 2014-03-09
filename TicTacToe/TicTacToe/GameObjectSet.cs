using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    /// <summary>
    /// Represents a set of multiple gaming objects to be drawn at the same time.
    /// There still may be an internal drawing order.
    /// </summary>
    class GameObjectSet : GameObject
    {
        LinkedList<GameObject> objects;

        public GameObjectSet(Game1 game) : base(game)
        {
            objects = new LinkedList<GameObject>();
        }
    }
}
