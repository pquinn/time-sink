using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Collisions
{
    public interface ICollideable
    {
        ICollisionGeometry CollisionGeometry { get; }
    }
}
