using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Physics
{
    public static class PhysicsConstants
    {
        public static Vector2 Gravity = new Vector2(0, 27.5f);
        public const float PhysicsTick = 1f / 60f;

        public static float PixelsPerMeter = 64f;

        public static int MetersToPixels(float meters)
        {
            return (int)(meters * PixelsPerMeter);
        }

        public static Vector2 MetersToPixels(Vector2 meters)
        {
            return meters * PixelsPerMeter;
        }

        public static float PixelsToMeters(int pixels)
        {
            return pixels / PixelsPerMeter;
        }

        public static Vector2 PixelsToMeters(Vector2 pixels)
        {
            return pixels / PixelsPerMeter;
        }
    }
}
