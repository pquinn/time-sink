using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Collisions
{
    public static class PolygonCollision
    {
        [Collided.Overload]
        public static CollisionInfo Collided(IPolygon polyA, IPolygon polyB)
        {
            return SeparatingAxisTheorem.PolygonCollision(polyA, polyB);
        }
    }
}
