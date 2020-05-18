using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine.GUI
{
    public class Checkbox : GUIElement
    {
        public Texture2D Box { get; set; }
        public bool Checked { get; set; }

        public override void Update()
        {
            if (InputManager.IsMouseReleased() && Bounds.Contains(InputManager.GetMousePosition()))
            {
                Checked = !Checked;
                OnAction();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            int width = Math.Min(Bounds.Width, Bounds.Height);
            spriteBatch.Draw(Box, new Rectangle(Bounds.X, Bounds.Y, width, width), Checked ? Color.Red : Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(Bounds.X + width, Bounds.Y), Color.Black);
        }
    }
}
