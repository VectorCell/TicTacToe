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
    public class SolidColorTexture : Texture2D
    {
        private Color color;
        // Gets or sets the color used to create the texture
        //public Color Color
        //{
        //    get { return _color; }
        //    set
        //    {
        //        if (value != _color)
        //        {
        //            _color = value;
        //            SetData<Color>(new Color[] { _color });
        //        }
        //    }
        //}

        public SolidColorTexture(Game1 game)
            : base(game.GraphicsDevice, 1, 1)
        {
            //default constructor
        }

        public SolidColorTexture(Game1 game, Color color)
            : base(game.GraphicsDevice, 1, 1)
        {
            this.color = color;
            this.SetData<Color>(new Color[] { color });
        }

        public SolidColorTexture(Game1 game, Color color, int width, int height)
            : base(game.GraphicsDevice, width, height)
        {
            this.color = color;
            this.SetData<Color>(new Color[] { color });
        }

    }
}
