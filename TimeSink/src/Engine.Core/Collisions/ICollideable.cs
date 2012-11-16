using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Dynamics;

namespace TimeSink.Engine.Core.Collisions
{
    public interface ICollideable : IPhysicsEnabledBody
    {
        List<Fixture> CollisionGeometry { get; }
    }
}
