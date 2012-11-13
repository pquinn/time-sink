using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;

namespace Engine.Game.Entities.Enemies
{
    class NormalCentipede : Entity, IHaveHealth
    {

        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Goomba";

        private List<DamageOverTimeEffect> dots;

        private float health;
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public override ICollisionGeometry CollisionGeometry
        {
            get { throw new NotImplementedException(); }
        }

        public NormalCentipede(Vector2 position)
        {
            health = 100;
            physics = new GravityPhysics(position, CENTIPEDE_MASS)
            {
                GravityEnabled = true
            };
            dots = new List<DamageOverTimeEffect>();
        }

        private GravityPhysics physics;
        public override IPhysicsParticle PhysicsController
        {
            get { throw new NotImplementedException(); }
        }

        public override IRendering Rendering
        {
            get { throw new NotImplementedException(); }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            throw new NotImplementedException();
        }

        public override void Load(EngineGame engineGame)
        {
            throw new NotImplementedException();
        }
    }
}
