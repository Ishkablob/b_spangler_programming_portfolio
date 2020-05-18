using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public class AStarNode
    {
        public AStarNode Parent { get; set; }
        public Vector3 Position { get; set; }
        public bool Passable { get; set; }
        public bool Closed { get; set; }
        public float Cost { get; set; }
        public float Heuristic { get; set; }
        public int Col { get; set; } // x
        public int Row { get; set; }
        public AStarNode(int col, int row, Vector3 position)
        {
            Col = col;
            Row = row;
            Position = position;// for 3D space

            Passable = true;
            Cost = 0;
            Heuristic = 0;
            Parent = null;
            Closed = false;
        }

    }
}
