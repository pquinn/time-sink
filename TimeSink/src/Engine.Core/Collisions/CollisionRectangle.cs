using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public class CollisionRectangle : APolygon
    {
        public Vector2 TopLeft     { get; protected set; }
        public Vector2 BottomLeft  { get; protected set; }
        public Vector2 BottomRight { get; protected set; }
        public Vector2 TopRight    { get; protected set; }

        public override IList<Vector2> Vertices
        {
            get
            {
                return new List<Vector2>() { TopLeft, BottomLeft, BottomRight, TopRight };
            }
        }

        public CollisionRectangle(Rectangle r)
        {
            TopLeft = new Vector2(r.Left, r.Top);
            BottomLeft = new Vector2(r.Left, r.Bottom);
            BottomRight = new Vector2(r.Right, r.Bottom);
            TopRight = new Vector2(r.Right, r.Top);
        }

        public CollisionRectangle(Vector2 topLeft, Vector2 bottomLeft, Vector2 bottomRight, Vector2 topRight)
        {
            TopLeft = topLeft;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            TopRight = topRight;
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
