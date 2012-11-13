using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Game.Entities;
using TimeSink.Engine.Game.Entities.Weapons;

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
            get
            {
                return new CollisionRectangle(
                    new Rectangle(
                        (int)physics.Position.X,
                        (int)physics.Position.Y,
                        32, 32
                    ));
            }
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
            get { return physics; }
        }

        public override IRendering Rendering
        {
            get
            {
                var tint = Math.Min(100, 2.55f * health);
                return new TintedRendering(
                  CENTIPEDE_TEXTURE,
                  physics.Position,
                  0,
                  Vector2.One,
                  new Color(255f, tint, tint, 255f));
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            throw new NotImplementedException();
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(WorldGeometry world, CollisionInfo info)
        {
            // Handle whether collision should disable gravity
            if (info.MinimumTranslationVector.Y > 0)
            {
                physics.GravityEnabled = false;
                physics.Velocity = new Vector2(physics.Velocity.X, Math.Min(0, physics.Velocity.Y));
            }
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Arrow arrow, CollisionInfo info)
        {
            health -= 25;
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Dart dart, CollisionInfo info)
        {
            RegisterDot(dart.dot);
        }

        public override void Update(GameTime time, EngineGame world)
        {
            if (health <= 0)
            {
                Console.WriteLine("goomba dead");
                Dead = true;
            }

            foreach (DamageOverTimeEffect dot in dots)
            {
                if (dot.Active)
                    health -= dot.Tick(time);
            }
            RemoveInactiveDots();


            if (Dead)
            {
                world.RenderManager.UnregisterRenderable(this);
                world.CollisionManager.UnregisterCollisionBody(this);
                world.PhysicsManager.UnregisterPhysicsBody(this);
            }
        }

        private void RemoveInactiveDots()
        {
            // there has to be a better way to do this.........
            List<DamageOverTimeEffect> newDots = new List<DamageOverTimeEffect>();
            foreach (DamageOverTimeEffect dot in dots)
            {
                if (!dot.Finished)
                    newDots.Add(dot);
            }
            dots = newDots;
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(CENTIPEDE_TEXTURE);
        }

        public void RegisterDot(DamageOverTimeEffect dot)
        {
            if (!dot.Active)
            {
                dots.Add(dot);
                dot.Active = true;
            }
        }
    }
}
