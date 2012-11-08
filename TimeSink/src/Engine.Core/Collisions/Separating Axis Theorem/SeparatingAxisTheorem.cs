using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public static class SeparatingAxisTheorem
    {
        #region Separating Axis Theorem
        // Calculate the projection of a polygon on an axis
        // and returns it as a [min, max] interval
        private static void ProjectPolygon(Vector2 axis, IPolygon poly, out float min, out float max)
        {
            max = min = Vector2.Dot(poly.Vertices[0], axis);

            foreach (var point in poly.Vertices.Skip(1))
            {
                var scalar = Vector2.Dot(point, axis);

                if (scalar > max)
                    max = scalar;
                else if (scalar < min)
                    min = scalar;
            }
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
        public static CollisionInfo PolygonCollision(IPolygon polyA, IPolygon polyB)//, Vector2 velocity) 
        {
            //Initialize results
            CollisionInfo result = new CollisionInfo();
            result.Intersect = true;

            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();

            // Loop through all the edges of both polygons
            foreach (Vector2 edge in polyA.Edges.Concat(polyB.Edges))
            {
                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA, minB, maxA, maxB;
                ProjectPolygon(axis, polyA, out minA, out maxA);
                ProjectPolygon(axis, polyB, out minB, out maxB);

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

                    Vector2 d = polyA.Center - polyB.Center;
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
