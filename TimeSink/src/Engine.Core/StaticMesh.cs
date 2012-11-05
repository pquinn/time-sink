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
    public class StaticMesh : ICollideable, IRenderable
    {
        const string texture = "Textures/Ground_Tile1";
        public StaticMesh(Vector2 position)
        {
            this.Position = position;
        }

        public Vector2 Position { get; set; }

        public ICollisionGeometry CollisionGeometry
        {
            get
            {
                return new AACollisionRectangle(
                  new Rectangle((int)Position.X, (int)Position.Y, 128, 128));
            }
        }

        public IRendering Rendering
        {
            get 
            {
                return new BasicRendering(texture, Position);
            }
        }
    }
}
