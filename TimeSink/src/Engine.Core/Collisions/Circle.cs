using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
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
}
