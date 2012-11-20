using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public class CollisionTriangle : APolygon
    {
        public Vector2[] TriangleVertices;

        public override IList<Vector2> Vertices
        {
            get
            {
                return TriangleVertices.ToList();
            }
        }

        public CollisionTriangle(Vector2 vertA, Vector2 vertB, Vector2 vertC)
        {
            TriangleVertices = new Vector2[] { vertA, vertB, vertC };
        }

        [Collided.Overload]
        public CollisionInfo Collided(CollisionSet s)
        {
            return s.Collided(this);
        }

        [Collided.Overload]
        public CollisionInfo Collided(CollisionCircle collisionCircle)
        {
            return collisionCircle.Collided(this);
        }
    }
}
