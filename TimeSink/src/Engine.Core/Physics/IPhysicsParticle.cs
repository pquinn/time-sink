using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Physics
{
    public interface IPhysicsParticle
    {
        Vector2 OldPosition { get; }
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        Vector2 Acceleration { get; set; }

        float Mass { get; set; }

        void Update(GameTime timeStep);
        void ApplyForce(Vector2 force);
    }
}
