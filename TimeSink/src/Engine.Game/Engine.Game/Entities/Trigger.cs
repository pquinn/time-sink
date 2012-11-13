using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Game.Entities;

namespace Engine.Game.Entities
{
    public delegate void TriggerDelegate(ICollideable collided);

    public class Trigger : Entity
    {
        public event TriggerDelegate Triggered;

        private ICollisionGeometry _geom;
        public override ICollisionGeometry CollisionGeometry
        {
            get { return _geom; }
        }

        public Trigger(ICollisionGeometry geom)
        {
            _geom = geom;
        }

        public override IPhysicsParticle PhysicsController
        {
            get { return null; }
        }

        public override IRendering Rendering
        {
            get { return null; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            
        }

        public override void Load(EngineGame engineGame)
        {
            
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(ICollideable obj, CollisionInfo info)
        {
            if (Triggered != null)
                Triggered(obj);
        }
    }
}
