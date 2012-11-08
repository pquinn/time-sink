using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core
{
    public class StaticMesh : Entity
    {
        string texture;
        public StaticMesh(string texture, Vector2 position)
        {
            this.texture = texture;
            this.Position = position;
        }

        public Vector2 Position { get; set; }

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
                return new BasicRendering(texture, Position);
            }
        }

        public override IPhysicsParticle PhysicsController
        {
            get { return null; }
        }

        public override void HandleKeyboardInput(GameTime gameTime)
        {
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(texture);
        }
    }
}
