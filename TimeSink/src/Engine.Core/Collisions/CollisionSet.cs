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
        public bool Collided(ICollisionGeometry cg)
        {
            return _geom.Any(x => Collisions.Collided.Invoke(x, cg));
        }
    }
}
