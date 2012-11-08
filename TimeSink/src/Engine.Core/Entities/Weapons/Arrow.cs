using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Entities.Weapons
{
    class Arrow : Entity
    {
        const float ARROW_MASS = 10f;
        private GravityPhysics physics;

        public Arrow(Vector2 position)
        {
            physics = new GravityPhysics(position, ARROW_MASS)
            {
                GravityEnabled = true
            };
        }

        public override ICollisionGeometry CollisionGeometry
        {
            get
            {
                return new CollisionRectangle(
                        new Rectangle(0, 0, 25, 25)
                        );  
            }
        }

        public override IPhysicsParticle PhysicsController
        {
            get { throw new NotImplementedException(); }
        }

        public override IRendering Rendering
        {
            get { throw new NotImplementedException(); }
        }

        public override void HandleKeyboardInput(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Load(EngineGame engineGame)
        {
            throw new NotImplementedException();
        }
    }
}
