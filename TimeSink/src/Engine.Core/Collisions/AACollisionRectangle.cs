using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public class AACollisionRectangle : ICollisionGeometry
    {
        public Rectangle Rect;

        public AACollisionRectangle(Rectangle r)
        {
            Rect = r;
        }

        [Collided.Overload]
        public bool Collided(AACollisionRectangle r)
        {
            return Rect.Intersects(r.Rect);
        }

        [Collided.Overload]
        public bool Collided(CollisionCircle c)
        {
            var rad = c.Circ.Radius;
            var center = c.Circ.Center;

            return c.Circ.ContainsPoint(new Point(Rect.Left, Rect.Top))
                || c.Circ.ContainsPoint(new Point(Rect.Left, Rect.Bottom))
                || c.Circ.ContainsPoint(new Point(Rect.Right, Rect.Top))
                || c.Circ.ContainsPoint(new Point(Rect.Right, Rect.Bottom))
                || Rect.ContainsPoint(new Point(center.X, center.Y - rad))
                || Rect.ContainsPoint(new Point(center.X, center.Y + rad))
                || Rect.ContainsPoint(new Point(center.X - rad, center.Y))
                || Rect.ContainsPoint(new Point(center.X + rad, center.Y));
        }

        [Collided.Overload]
        public bool Collided(CollisionSet s)
        {
            return s.Collided(this);
        }
    }
}
