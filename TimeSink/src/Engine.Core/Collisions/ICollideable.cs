using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Engine.Core.Collisions
{
    public interface ICollideable : IPhysicsEnabledBody
    {
        List<FarseerPhysics.Dynamics.Fixture> CollisionGeometry { get; }
    }
}
