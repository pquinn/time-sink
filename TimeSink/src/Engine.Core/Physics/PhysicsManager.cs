using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.DebugViews;

namespace TimeSink.Engine.Core.Physics
{
    public class PhysicsManager
    {
        //private HashSet<IPhysicsEnabledBody> bodies = new HashSet<IPhysicsEnabledBody>();
        public World World { get; private set; }

        public PhysicsManager()
        {
            World = new World(PhysicsConstants.Gravity);
        }

        public void RegisterPhysicsBody(IPhysicsEnabledBody body)
        {
            body.InitializePhysics(World);
        }

        public void Update(GameTime gameTime)
        {
            World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
