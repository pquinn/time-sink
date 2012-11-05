using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public struct AACollisionRectangle : ICollisionGeometry
    {
        public Rectangle Rect;

        public AACollisionRectangle(Rectangle r)
        {
            Rect = r;
        }

        [Collided.Overload]
        public CollisionInfo Collided(AACollisionRectangle r)
        {
            return new CollisionInfo()
            {
                Intersect = Rect.Intersects(r.Rect)
            };
        }

        [Collided.Overload]
        public CollisionInfo Collided(CollisionCircle c)
        {
            var rad = c.Circ.Radius;
            var center = c.Circ.Center;
            var result = c.Circ.ContainsPoint(new Vector2(Rect.Left, Rect.Top))
                || c.Circ.ContainsPoint(new Vector2(Rect.Left, Rect.Bottom))
                || c.Circ.ContainsPoint(new Vector2(Rect.Right, Rect.Top))
                || c.Circ.ContainsPoint(new Vector2(Rect.Right, Rect.Bottom))
                || Rect.ContainsPoint(new Vector2(center.X, center.Y - rad))
                || Rect.ContainsPoint(new Vector2(center.X, center.Y + rad))
                || Rect.ContainsPoint(new Vector2(center.X - rad, center.Y))
                || Rect.ContainsPoint(new Vector2(center.X + rad, center.Y));

            return new CollisionInfo()
            {
                Intersect = result
            };
        }

        [Collided.Overload]
        public CollisionInfo Collided(CollisionSet s)
        {
            return s.Collided(this);
        }

        [Collided.Overload]
        public CollisionInfo Collided(CollisionRectangle r)
        {
            return r.Collided(new CollisionRectangle(this.Rect));
        }
    }
}
