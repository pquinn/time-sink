using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public class CollisionRectangle : ICollisionGeometry
    {
        public Vector2 TopLeft     { get; protected set; }
        public Vector2 BottomLeft  { get; protected set; }
        public Vector2 BottomRight { get; protected set; }
        public Vector2 TopRight    { get; protected set; }

        public IEnumerable<Vector2> Vertices
        {
            get
            {
                return new List<Vector2>() { TopLeft, BottomLeft, BottomRight, TopRight };
            }
        }

        public IEnumerable<Vector2> Edges
        {
            get
            {
                return new List<Vector2>()
                {
                    BottomLeft - TopLeft,
                    BottomRight - BottomLeft,
                    TopRight - BottomLeft,
                    TopLeft - TopRight
                };
            }
        }

        public Vector2 Center 
        {
            get 
            {
                return TopLeft + TopRight + BottomLeft + BottomRight / 4;
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
        public CollisionInfo Collided(CollisionRectangle r)
        {
            return RectangleCollision(this, r);
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

        #region Separating Axis Theorem
        // Calculate the projection of a polygon on an axis
        // and returns it as a [min, max] interval
        private static void ProjectRectangle(Vector2 axis, CollisionRectangle rect, out float min, out float max) 
        {
            max = min = Vector2.Dot(rect.TopLeft, axis);

            var scalar = Vector2.Dot(rect.BottomLeft, axis);

            if (scalar > max)
                max = scalar;
            else if (scalar < min)
                min = scalar;

            scalar = Vector2.Dot(rect.BottomRight, axis);

            if (scalar > max)
                max = scalar;
            else if (scalar < min)
                min = scalar;

            scalar = Vector2.Dot(rect.TopRight, axis);

            if (scalar > max)
                max = scalar;
            else if (scalar < min)
                min = scalar;
        }

        // Calculate the distance between [minA, maxA] and [minB, maxB]
        // The distance will be negative if the intervals overlap
        private static float IntervalDistance(float minA, float maxA, float minB, float maxB) 
        {
            if (minA < minB)
                return minB - maxA;
            else
                return minA - maxB;
        }

        // Check if polygon A is going to collide with polygon B.
        // The last parameter is the *relative* velocity 
        // of the polygons (i.e. velocityA - velocityB)
        private static CollisionInfo RectangleCollision(CollisionRectangle rectA, CollisionRectangle rectB)//, Vector2 velocity) 
        {
            //Initialize results
            CollisionInfo result = new CollisionInfo();
            result.Intersect = true;
            
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();

            // Loop through all the edges of both polygons
            foreach (Vector2 edge in rectA.Edges.Concat(rectB.Edges)) 
            {
                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA, minB, maxA, maxB;
                ProjectRectangle(axis, rectA, out minA, out maxA);
                ProjectRectangle(axis, rectB, out minB, out maxB);

                // Check if the polygon projections are currentlty intersecting
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0)
                    result.Intersect = false;

                // ===== 2. Now find if the polygons *will* intersect =====

                // Project the velocity on the current axis
                //float velocityProjection = axis.DotProduct(velocity);

                // Get the projection of polygon A during the movement
                //if (velocityProjection < 0)
                //    minA += velocityProjection;
                //else
                //    maxA += velocityProjection;

                // Do the same test as above for the new projection
                //float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                //if (intervalDistance > 0) 
                //    result.WillIntersect = false;

                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result.Intersect) break; // && !result.WillIntersect) break;

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance) 
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = rectA.Center - rectB.Center;
                    if (Vector2.Dot(d, translationAxis) < 0)
                        translationAxis = -translationAxis;
                }
            }

            // The minimum translation vector
            // can be used to push the polygons appart.
            if (result.Intersect)
                result.MinimumTranslationVector = 
                       translationAxis * minIntervalDistance;
            
            return result;
        }
        #endregion
    }
}
