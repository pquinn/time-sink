using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public static class ContainsPointExtensions
    {
        public static bool ContainsPoint(this Circle c, Vector2 p)
        {
            return p.X <= c.Center.X + c.Radius
                && p.X >= c.Center.X - c.Radius
                && p.Y <= c.Center.Y + c.Radius
                && p.Y >= c.Center.Y - c.Radius;
        }

        public static bool ContainsPoint(this Rectangle r, Vector2 p)
        {
            return p.X <= r.Right && p.X >= r.Left
                && p.Y <= r.Bottom && p.Y >= r.Top;
        }
    }
}
