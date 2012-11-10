using Microsoft.Xna.Framework;

using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Game.Entities.Weapons
{
    public class Arrow : Entity
    {
        const float ARROW_MASS = 1f;
        const string ARROW_TEXTURE_NAME = "Textures/Weapons/Arrow";

        public GravityPhysics physics { get; private set; }

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
                return new CollisionRectangle(new Rectangle(
                    (int)physics.Position.X,
                    (int)physics.Position.Y,
                    64,
                    32
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
                    ARROW_TEXTURE_NAME,
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
                Dead = true;
            }
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(ARROW_TEXTURE_NAME);
        }

        public override void Update(GameTime time, EngineGame world)
        {
            base.Update(time, world);

            if (Dead)
            {
                world.RenderManager.UnregisterRenderable(this);
                world.CollisionManager.UnregisterCollisionBody(this);
                world.PhysicsManager.UnregisterPhysicsBody(this);
            }
        }
    }
}
