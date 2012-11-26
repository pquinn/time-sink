using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.DebugViews;
using Autofac;

namespace TimeSink.Engine.Core.Physics
{
    public class PhysicsManager
    {
        private IContainer engineRegistrations;

        //private HashSet<IPhysicsEnabledBody> bodies = new HashSet<IPhysicsEnabledBody>();
        public World World { get; private set; }

        public PhysicsManager(IContainer engineRegistrations)
        {
            World = engineRegistrations.Resolve<World>();
            this.engineRegistrations = engineRegistrations;
        }

        public void RegisterPhysicsBody(IPhysicsEnabledBody body)
        {
            body.InitializePhysics(engineRegistrations);
        }

        public void Update(GameTime gameTime)
        {
            World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
