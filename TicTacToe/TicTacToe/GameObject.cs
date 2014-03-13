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

namespace TicTacToe
{
    public class GameObject
    {
        protected Game1 game;

        protected Texture2D texture;
        protected Rectangle drawRect;
        protected Color color;

        public GameObject(Game1 game)
        {
            this.game = game;
            this.texture = game.Content.Load<Texture2D>("blank");
            this.drawRect = new Rectangle(0, 0, texture.Width, texture.Height);
            this.color = Color.White;
        }

        public GameObject(Game1 game, Rectangle drawRect)
        {
            this.game = game;
            this.texture = game.Content.Load<Texture2D>("blank");
            this.drawRect = drawRect;
            this.color = Color.White;
        }

        public GameObject(Game1 game, Texture2D texture)
        {
            this.game = game;
            this.texture = texture;
            this.drawRect = new Rectangle(0, 0, texture.Width, texture.Height);
            this.color = Color.White;
        }

        public GameObject(Game1 game, Texture2D texture, Rectangle drawRect)
        {
            this.game = game;
            this.texture = texture;
            this.drawRect = drawRect;
            this.color = Color.White;
        }

        public virtual void Initialize()
        {

        }

        public virtual void LoadContent()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.drawRect, color);
        }

        public virtual void Update()
        {

        }

        public virtual void Update(MouseState lastMouse, MouseState currentMouse)
        {
            
        }
    }
}
