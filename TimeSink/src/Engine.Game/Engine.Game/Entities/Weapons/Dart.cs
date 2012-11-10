using Microsoft.Xna.Framework;

using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

using TimeSink.Engine.Game.Entities;
using System;
 

namespace TimeSink.Engine.Game.Entities.Weapons
{
    public class Dart : Entity
    {
        const float DART_MASS = 1f;
        const string DART_TEXTURE_NAME = "Textures/Weapons/Dart";

        public GravityPhysics physics { get; private set; }
        public DamageOverTimeEffect dot { get; private set; }

        public Dart(Vector2 position)
        {
            physics = new GravityPhysics(position, DART_MASS)
            {
                GravityEnabled = true
            };

            dot = new DamageOverTimeEffect(4, 100);
        }

        public override ICollisionGeometry CollisionGeometry
        {
            get
            {
                return new CollisionRectangle(new Rectangle(
                    (int)physics.Position.X,
                    (int)physics.Position.Y,
                    16,
                    8
                ));
            }
        }

        public override IPhysicsParticle PhysicsController
        {
            get { return physics; }
        }

        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(
                    DART_TEXTURE_NAME,
                    physics.Position,
                    0,
                    Vector2.One
                );
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(WorldGeometry entity, CollisionInfo info)
        {
            Dead = true;
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Entity entity, CollisionInfo info)
        {
            if (!(entity is UserControlledCharacter))
            {
                Console.WriteLine("dart is now dead");
                Dead = true;
            }
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(DART_TEXTURE_NAME);
        }

        public override void Update(GameTime time, EngineGame world)
        {
            base.Update(time, world);

            if (Dead)
            {
                Console.WriteLine("dart deregistered");
                world.RenderManager.UnregisterRenderable(this);
                world.CollisionManager.UnregisterCollisionBody(this);
                world.PhysicsManager.UnregisterPhysicsBody(this);
            }
        }
    }
}
