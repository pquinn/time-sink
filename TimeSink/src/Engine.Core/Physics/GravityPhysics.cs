using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Physics
{
    public class GravityPhysics : IPhysicsParticle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public float Mass { get; set; }

        public GravityPhysics(Vector2 p, float m)
            : this(p, Vector2.Zero, Vector2.Zero, m)
        { }

        public GravityPhysics(Vector2 p, Vector2 v, Vector2 a, float m)
        {
            Mass = m;
            Position = p;
            Velocity = v;
            Acceleration = a;
        }

        bool _gravity;
        public bool GravityEnabled
        {
            get
            {
                return _gravity;
            }
            set
            {
                if (_gravity == value)
                    return;

                _gravity = value;
                if (_gravity)
                    Acceleration += PhysicsConstants.Gravity;
                else
                    Acceleration -= PhysicsConstants.Gravity;
            }
        }

        public void Update(GameTime timeStep)
        {
            var time = (float)timeStep.ElapsedGameTime.TotalSeconds;
            Position += Velocity * time;
            Velocity += Acceleration * time;
        }

        public void ApplyForce(Vector2 force)
        {
            Acceleration += force / Mass;
        }
    }
}
