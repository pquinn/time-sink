using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Collisions
{
    public class CollisionCircle : ICollisionGeometry
    {
        public Circle Circ;

        public CollisionCircle(Circle c)
        {
            Circ = c;
        }

        [Collided.Overload]
        public bool Collided(CollisionCircle c)
        {
            return c.Circ.Intersects(Circ);
        }

        [Collided.Overload]
        public bool Collided(CollisionRectangle r)
        {
            return r.Collided(this);
        }

        [Collided.Overload]
        public bool Collided(CollisionSet s)
        {
            return s.Collided(this);
        }
    }
}
