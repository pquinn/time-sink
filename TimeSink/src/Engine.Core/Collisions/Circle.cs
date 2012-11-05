using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Microsoft.Xna.Framework
{
    public struct Circle
    {
        public float Radius;
        public Vector2 Center;

        public Circle(float x, float y, float r)
        {
            Radius = r;
            Center = new Vector2(x, y);
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
