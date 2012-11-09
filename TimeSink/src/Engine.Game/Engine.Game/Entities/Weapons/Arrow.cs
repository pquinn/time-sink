using Microsoft.Xna.Framework;

using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Game.Entities.Weapons
{
    public class Arrow : Entity
    {
        const float ARROW_MASS = 10f;
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
                    25,
                    25
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
        public void OnCollidedWith(WorldGeometry world, CollisionInfo info)
        {
            // Handle whether collision should disable gravity
            if (info.MinimumTranslationVector.Y > 0)
            {
                //physics.GravityEnabled = false;
                //physics.Velocity = new Vector2(physics.Velocity.X, Math.Min(0, physics.Velocity.Y));
            }
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(ARROW_TEXTURE_NAME);
        }
    }
}
