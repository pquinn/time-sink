using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core
{
    public class StaticMesh : ICollideable
    {
        Point position;

        public StaticMesh(Point position)
        {
            this.position = position;
        }

        public ICollisionGeometry CollisionGeometry
        {
            get
            {
                return new AACollisionRectangle(
                  new Rectangle(position.X, position.Y, 128, 128));
            }
        }
    }
}
