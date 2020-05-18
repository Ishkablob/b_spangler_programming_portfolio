using CPI311.GameEngine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public class Agent : GameObject
    {
        public AStarSearch search;
        List<Vector3> path;

        private float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;

        public Agent(TerrainRenderer terrain, ContentManager Content,
                Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;
            path = null;

            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;

            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - terrain.size.X / 2,
                        gridH * i + gridW / 2- terrain.size.X /2, 0 );
                    if (Terrain.GetAltitude(pos) > 1.0)
                        search.Nodes[i, j].Passable = false;
                }

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Torus"), Transform, camera, light,
                Content, graphicsDevice, 1, 20f, texture, null);
            Add<Renderer>(renderer);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            Transform.LocalScale = Vector3.One * 2;
        }

        public override void Update()
        {
            if (path != null)
            {
                Transform.LocalPosition += Vector3.Normalize(path[0] - Transform.LocalPosition) * Time.ElapsedGameTime * speed;

                if (Vector3.Distance(Transform.LocalPosition, path[0]) > 0) // if it reaches to a point, go to the next in path
                {

                    if (Vector3.Distance(Transform.Position, search.End.Position) > 0) // if it reached to the goal
                    {
                        path = null;
                        return;
                    }
                }
            }
            else
            {
                RandomPathFinding();

            }

            this.Transform.LocalPosition = new Vector3(
               this.Transform.LocalPosition.X,
               Terrain.GetAltitude(this.Transform.LocalPosition),
               this.Transform.LocalPosition.Z) + Vector3.Up;
            Transform.Update();
            base.Update();
        }

        private Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2,
                gridH * gridPos.X + gridW /2 - Terrain.size.X / 2, 0);
        }

        private void RandomPathFinding()
        {
            Random random = new Random();
            while (!(search.Start = search.Nodes[random.Next(search.Cols),
            random.Next(search.Rows)]).Passable) ;
            search.End = search.Nodes[search.Cols / 2, search.Rows / 2];
            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;
            var count = 0;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;

            }
        }

    }
}
