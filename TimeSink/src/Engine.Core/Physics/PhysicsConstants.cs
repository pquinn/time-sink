using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

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

        /// <summary>
        /// Sets the velocity of a body instantly while preserving realistic physics simulation.
        /// </summary>
        /// <param name="body">Body to set velocity of.</param>
        /// <param name="velocity">Velocity to set body to.</param>
        public static void VelocityByForce(this Body body, Vector2 velocity)
        {
            var velChange = velocity - body.LinearVelocity;
            var impulse = body.Mass * velChange;
            body.ApplyLinearImpulse(impulse);
        }

        /// <summary>
        /// Accelerates the given body to the given target velocity.
        /// </summary>
        /// <param name="body">Body to accelerate.</param>
        /// <param name="targetVelocity">Velocity body is accelerating toward.</param>
        /// <param name="accel">Amount to accelerate. Must be positive.</param>
        public static void AccelerateToTargetVelocity(this Body body, Vector2 targetVelocity, float accel)
        {
            if (accel < 0)
                throw new ArgumentException("Expected a positive value for acceleration amount.", "accel");

            var vel = body.LinearVelocity;
            var desiredVel = new Vector2(
                vel.X < targetVelocity.X
                    ? Math.Min(vel.X + accel, targetVelocity.X)
                    : Math.Max(vel.X - accel, targetVelocity.X),
                vel.Y < targetVelocity.Y
                    ? Math.Min(vel.Y + accel, targetVelocity.Y)
                    : Math.Max(vel.Y - accel, targetVelocity.Y));

            body.VelocityByForce(desiredVel);
        }
    }
}
