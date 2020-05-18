using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class AnimatedSprite : Sprite
    {
        public int Frames { get; set; }
        public float Frame { get; set; }
        public float Speed { get; set; }
        public int Row { get; set; }

        public AnimatedSprite(Texture2D texture, int frames = 1) : base (texture)
        {
            Frames = frames;
            Frame = 0;
            Speed = 1;
        }

        public override void Update()
        {
            Frame += Speed * 32;
            if (Frame >= 256)
                Frame = 0;
            Source = new Rectangle((int)Frame, Row, Texture.Width / Frames, 32);
        }
    }
}
