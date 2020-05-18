using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine.GUI
{
    public class Button : GUIElement
    {

        public override void Update()
        {
            if (InputManager.IsMouseReleased() && Bounds.Contains(InputManager.GetMousePosition()))
                OnAction();
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, Text, new Vector2(Bounds.X, Bounds.Y), Color.Black);
        }
    }
}
