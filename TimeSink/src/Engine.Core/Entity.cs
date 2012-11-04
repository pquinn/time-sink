using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Engine.Core
{
    public abstract class Entity
        : ICollideable, IPhysicsEnabledBody, IKeyboardControllable
    {
        public abstract void Update(GameTime time, Game world)
        {

        }
    }
}