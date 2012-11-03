using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Physics
{
    public class PhysicsManager
    {
        private HashSet<IPhysicsEnabledBody> bodies = new HashSet<IPhysicsEnabledBody>();

        public bool RegisterPhysicsBody(IPhysicsEnabledBody body)
        {
            return bodies.Add(body);
        }

        public bool UnregisterPhysicsBody(IPhysicsEnabledBody body)
        {
            return bodies.Remove(body);
        }

        public void Update(GameTime gameTime) 
        {
            foreach (var body in bodies)
                body.PhysicsController.Update(gameTime);
        }
    }
}
