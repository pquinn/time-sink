using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Autofac;

namespace TimeSink.Engine.Core.Physics
{
    public interface IPhysicsEnabledBody
    {
        void InitializePhysics(IContainer engineRegistrations);
    }
}
