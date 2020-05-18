using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Light : Component
    {
        public static Light Current { get; set; }
        public static Light Default { get; set; }

        public Color Ambient { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }

        public Light()
        {
            Ambient = Color.White;
            Diffuse = Color.White;
            Specular = Color.White;
        }

        static Light()
        {
            Default = new Light();
            Default.Transform = new Transform();
            Current = Default;
        }
    }

}
