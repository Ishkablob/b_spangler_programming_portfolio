using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public interface IUpdateable { void Update(); }
    public interface IRenderable { void Draw(); }
    public interface IDrawable { void Draw(SpriteBatch spriteBatch); }
}
