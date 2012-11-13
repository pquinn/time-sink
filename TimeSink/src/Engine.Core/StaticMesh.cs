using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace TimeSink.Engine.Core
{
    public class StaticMesh : Entity
    {
        string texture;
        public StaticMesh(string texture, Vector2 position, float rotation, Vector2 scale)
        {
            this.texture = texture;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public override ICollisionGeometry CollisionGeometry
        {
            get
            {
                return new AACollisionRectangle(
                  new Rectangle((int)Position.X, (int)Position.Y, 128, 128));
            }
        }

        public override IRendering Rendering
        {
            get 
            {
                return new BasicRendering(texture, Position, Rotation, Scale);
            }
        }

        public override IPhysicsParticle PhysicsController
        {
            get { return null; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(texture);
        }

        internal void Expand(IResourceCache<Texture2D> cache, Vector2 dragOffset, Vector2 origScale, Matrix transform)
        {
            var tex = cache.GetResource(texture);            
            var size = origScale * new Vector2(tex.Width, tex.Height);

            var relativeTransform =
                Matrix.CreateRotationZ(Rotation) *
                transform;

            var offsetInMeshFrame = Vector2.Transform(dragOffset, relativeTransform);
            
            Scale += (2 * offsetInMeshFrame) / size;
        }
    }
}
