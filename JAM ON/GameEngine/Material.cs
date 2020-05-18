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
    public class Material
    {
        public Effect effect;
        public Texture2D DiffuseTexture;
        public float Shininess;
        public int Passes { get { return effect.CurrentTechnique.Passes.Count; } }
        public int CurrentTechnique { get; set; }
        public Vector3 AmbientColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Matrix World { get; set; }
        public Camera Camera { get; set; }
        public Light Light { get; set; }

      public Material(Matrix world, Camera camera, Light light, ContentManager content, string filename,
          int technique, float shininess, Texture2D texture)
      {
            effect = content.Load<Effect>(filename);
            World = world;
            Camera = camera;
            Light = light;
            CurrentTechnique = technique;
            DiffuseTexture = texture;
            AmbientColor = Color.LightGray.ToVector3();
            SpecularColor = Color.LightGray.ToVector3();
            DiffuseColor = Color.LightGray.ToVector3();
      }
      public virtual void Apply(int currentPass)
      {
            effect.CurrentTechnique = effect.Techniques[CurrentTechnique];
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(Camera.View);
            effect.Parameters["Projection"].SetValue(Camera.Projection);
            effect.Parameters["CameraPosition"].SetValue(Camera.Transform.Position);
            effect.Parameters["Shininess"].SetValue(Shininess);
            effect.Parameters["LightPosition"].SetValue(Light.Transform.Position);
            effect.Parameters["AmbientColor"].SetValue(AmbientColor);
            effect.Parameters["SpecularColor"].SetValue(SpecularColor);
            effect.Parameters["DiffuseColor"].SetValue(DiffuseColor);
            effect.Parameters["DiffuseTexture"].SetValue(DiffuseTexture);
            effect.CurrentTechnique.Passes[currentPass].Apply();
      }

    }

}
