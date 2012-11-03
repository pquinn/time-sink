using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Physics
{
    public interface IPhysicsEnabledBody
    {
        IPhysicsParticle PhysicsController { get; }
    }
}
