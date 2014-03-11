using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace TicTacToe.GameGridUtil
{
    public class MatchLocation
    {
        LinkedList<Vector2> vectors;

        public MatchLocation(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            vectors = new LinkedList<Vector2>();
            vectors.AddLast(v1);
            vectors.AddLast(v2);
            vectors.AddLast(v3);
        }

        public MatchLocation(LinkedList<Vector2> vectors)
        {
            this.vectors = new LinkedList<Vector2>();
            foreach (Vector2 vector in vectors)
            {
                this.vectors.AddLast(vector);
            }
        }

        public LinkedList<Vector2> GetVectors()
        {
            LinkedList<Vector2> vectors = new LinkedList<Vector2>();
            foreach (Vector2 vector in this.vectors)
            {
                vectors.AddLast(vector);
            }
            return vectors;
        }

        public Vector2[] GetVectorsAsArray()
        {
            return vectors.ToArray();
        }
    }
}
