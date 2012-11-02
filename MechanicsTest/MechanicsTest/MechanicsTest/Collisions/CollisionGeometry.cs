using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Microsoft.Xna.Framework
{
    public struct Circle
    {
        public int Radius;
        public Point Center;

        public Circle(int x, int y, int r)
        {
            Radius = r;
            Center = new Point(x, y);
        }

        public bool Intersects(Circle other)
        {
            var d2 = Vector2.DistanceSquared(
                new Vector2(other.Center.X, other.Center.Y),
                new Vector2(Center.X, Center.Y)
            );

            var radSum = Radius + other.Radius;

            return d2 < radSum * radSum;
        }
    }

    public static class ContainsPointExtensions
    {
        public static bool ContainsPoint(this Circle c, Point p)
        {
            return p.X <= c.Center.X + c.Radius
                && p.X >= c.Center.X - c.Radius
                && p.Y <= c.Center.Y + c.Radius
                && p.Y >= c.Center.Y - c.Radius;
        }

        public static bool ContainsPoint(this Rectangle r, Point p)
        {
            return p.X <= r.Right && p.X >= r.Left
                && p.Y <= r.Bottom && p.Y >= r.Top;
        }
    }
}

namespace MechanicsTest.Collisions
{
    public struct CollisionInfo
    {
        public static CollisionInfo NoCollision = new CollisionInfo();
    }

    public interface ICollisionGeometry
    {
    }

    public class CollisionRectangle : ICollisionGeometry
    {
        public Rectangle Rect;

        public CollisionRectangle(Rectangle r)
        {
            Rect = r;
        }

        [Collided.Overload]
        public bool Collided(CollisionRectangle r)
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

    public class CollisionSet : ICollisionGeometry
    {
        private HashSet<ICollisionGeometry> _geom = new HashSet<ICollisionGeometry>();
        public HashSet<ICollisionGeometry> Geometry
        {
            get { return _geom; }
        }

        [Collided.Overload]
        public bool Collided(ICollisionGeometry cg)
        {
            return _geom.Any(x => MechanicsTest.Collisions.Collided.Invoke(x, cg));
        }
    }
}
