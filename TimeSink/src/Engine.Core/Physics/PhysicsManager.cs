﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.DebugViews;
using Autofac;
using FarseerPhysics.Collision.Shapes;

namespace TimeSink.Engine.Core.Physics
{
    public class PhysicsManager
    {
        private IComponentContext engineRegistrations;

        public World World { get; private set; }

        public PhysicsManager(IComponentContext engineRegistrations)
        {
            World = new World(PhysicsConstants.Gravity);
            this.engineRegistrations = engineRegistrations;
        }

        public void RegisterPhysicsBody(IPhysicsEnabledBody body)
        {
            body.InitializePhysics(false, engineRegistrations);
        }

        public void Update(GameTime gameTime)
        {
            World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        internal void Clear()
        {
            World = new World(PhysicsConstants.Gravity);
        }

        public void UnregisterPhysicsBody(IPhysicsEnabledBody body)
        {
            body.DestroyPhysics();
        }
    }
}
