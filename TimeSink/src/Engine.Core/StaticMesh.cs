using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Engine.Core
{
    public class StaticMesh : ICollideable
    {
        public StaticMesh(Vector2 position)
        {
            this.Position = position;
        }

        public Vector2 Position { get; set; }

        public ICollisionGeometry CollisionGeometry
        {
            get
            {
                return new CollisionRectangle(
                  new Rectangle(
                      (int)Position.X, 
                      (int)Position.Y, 
                      128, 128));
            }
        }
    }
}
