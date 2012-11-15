using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2(this Vector3 vec3)
        {
            return new Vector2(vec3.X, vec3.Y);
        }

        public static Vector3 Plus(this Vector3 vec3, Vector2 vec2)
        {
            return new Vector3(vec3.X + vec2.X, vec3.Y + vec2.Y, vec3.Z);
        }

        public static Vector3 Plus(this Vector2 vec2, Vector3 vec3)
        {
            return new Vector3(vec3.X + vec2.X, vec3.Y + vec2.Y, vec3.Z);
        }

        public static Vector2 Plus(this Vector2 vec2, Point p)
        {
            return new Vector2(vec2.X + p.X, vec2.Y + p.Y);
        }

        public static Vector2 ToVec2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static Vector2 GetSurfaceNormal(this Vector2 vector)
        {
            var normal = new Vector2(vector.Y, -vector.X);
            normal.Normalize();

            return normal;
        }

        public static Vector2 DeNormalize(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static double ExtractZRotation(this Quaternion quat)
        {
            return Math.Atan2(
                2 * (quat.W * quat.Z + quat.X * quat.Y),
                1 - 2 * (Math.Pow(quat.Y, 2) + Math.Pow(quat.Z, 2)));
        }
    }
}
