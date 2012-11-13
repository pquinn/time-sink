using Engine.Game.Entities;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using System;

namespace TimeSink.Engine.Game.Entities.Weapons
{
    public class Arrow : Entity, IWeapon
    {
        const float ARROW_MASS = 1f;
        const string ARROW_TEXTURE_NAME = "Textures/Weapons/Arrow";

        const float MAX_ARROW_HOLD = 1;
        const float MIN_ARROW_INIT_SPEED = 500;
        const float MAX_ARROW_INIT_SPEED = 1500;

        public GravityPhysics physics { get; private set; }

        public Arrow() { }

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
                    (float)Math.Atan2(physics.Velocity.Y, physics.Velocity.X),
                    Vector2.One
                );
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        //[OnCollidedWith.Overload]
        //public void OnCollidedWith(WorldGeometry entity, CollisionInfo info)
        //{
        //    Dead = true;
        //}

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Entity entity, CollisionInfo info)
        {
            if (!(entity is UserControlledCharacter || entity is Trigger))
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

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime)
        {
            character.InHold = false;
            Arrow arrow = new Arrow(
                new Vector2(character.PhysicsController.Position.X + UserControlledCharacter.X_OFFSET,
                            character.PhysicsController.Position.Y + UserControlledCharacter.Y_OFFSET));
            var elapsedTime = Math.Min(gameTime.TotalGameTime.TotalSeconds - holdTime, MAX_ARROW_HOLD);
            // linear interp: y = 500 + (x - 0)(1300 - 500)/(MAX_HOLD-0) x = elapsedTime
            float speed =
                MIN_ARROW_INIT_SPEED + (MAX_ARROW_INIT_SPEED - MIN_ARROW_INIT_SPEED) /
                                       MAX_ARROW_HOLD *
                                       (float)elapsedTime;
            Vector2 initialVelocity = speed * character.Direction;
            arrow.physics.Velocity += initialVelocity;
            world.Entities.Add(arrow);
            world.RenderManager.RegisterRenderable(arrow);
            world.PhysicsManager.RegisterPhysicsBody(arrow);
            world.CollisionManager.RegisterCollisionBody(arrow);
        }
    }
}
