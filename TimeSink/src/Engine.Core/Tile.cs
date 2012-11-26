using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Xml.Serialization;

namespace TimeSink.Engine.Core
{
    public class Tile : Entity
    {
        const string EDITOR_NAME = "Tile";

        public Tile()
        {
        }

        public Tile(string texture, Vector2 position, float rotation, Vector2 scale)
        {
            this.Texture = texture;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }

        public string Texture { get; set; }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public Vector2 Scale { get; set; }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [XmlIgnore]
        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return new List<Fixture>();
            }
        }

        public override void InitializePhysics(World world)
        {
        }

        [XmlIgnore]
        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(Texture, Position, Rotation, Scale);
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(Texture);
        }


        public void Expand(IResourceCache<Texture2D> cache, Vector2 dragOffset, Vector2 origScale, Matrix transform)
        {
            var tex = cache.GetResource(Texture);

            var size = origScale * new Vector2(tex.Width / 2, tex.Height / 2) * 2;

            var relativeTransform =
                Matrix.CreateRotationZ(Rotation) *
                transform;

            var offsetInMeshFrame = Vector2.Transform(dragOffset, relativeTransform);

            Scale += (2 * offsetInMeshFrame) / size;
        }
    }
}
