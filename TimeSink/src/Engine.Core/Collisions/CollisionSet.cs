using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;

namespace TimeSink.Engine.Core.Collisions
{
    public class CollisionSet : ICollisionGeometry
    {
        private HashSet<ICollisionGeometry> _geom = new HashSet<ICollisionGeometry>();
        public HashSet<ICollisionGeometry> Geometry
        {
            get { return _geom; }
        }

        [Collided.Overload]
        public CollisionInfo Collided(ICollisionGeometry cg)
        {
            CollisionInfo result;
            foreach (var g in _geom)
            {
                result = Collisions.Collided.Invoke(g, cg);
                if (result.Intersect)
                    return result;
            }
            return CollisionInfo.NoCollision;
        }
    }
}
