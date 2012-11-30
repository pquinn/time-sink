using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using FarseerPhysics.Dynamics;
using System.Collections.Generic;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Collisions;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Objects;

namespace TimeSink.Entities.Weapons
{
    [SerializableEntity("16b8d25a-25f1-4b0b-acae-c60114aade0e")]
    public class Arrow : Entity, IWeapon
    {
        const float ARROW_MASS = 1f;
        const string ARROW_TEXTURE_NAME = "Textures/Weapons/Arrow";
        const string EDITOR_NAME = "Arrow";

        const float MAX_ARROW_HOLD = 1;
        const float MIN_ARROW_INIT_SPEED = 50;
        const float MAX_ARROW_INIT_SPEED = 150;

        private static readonly Guid GUID = new Guid("16b8d25a-25f1-4b0b-acae-c60114aade0e");

        private Vector2 _initialPosition;

        public Arrow()
            : this(Vector2.Zero)
        {
        }

        public Arrow(Vector2 position)
        {
            _initialPosition = position;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(
                    ARROW_TEXTURE_NAME,
                    PhysicsConstants.MetersToPixels(Physics.Position),
                    (float)Math.Atan2(Physics.LinearVelocity.Y, Physics.LinearVelocity.X),
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
        public bool OnCollidedWith(Entity entity, Contact info)
        {
            if (!(entity is UserControlledCharacter || entity is Trigger || entity is Ladder))
            {
                Dead = true;
            }
            return true;
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            textureCache.LoadResource(ARROW_TEXTURE_NAME);
        }

        public override void Update(GameTime time, EngineGame world)
        {
            base.Update(time, world);

            if (Dead)
            {
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                world.LevelManager.CollisionManager.UnregisterCollideable(this);
            }
        }

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime)
        {
            Arrow arrow = new Arrow(
                new Vector2(character.Physics.Position.X + UserControlledCharacter.X_OFFSET,
                            character.Physics.Position.Y + UserControlledCharacter.Y_OFFSET));

            world.LevelManager.RegisterEntity(arrow);

            character.InHold = false;

            var elapsedTime = Math.Min(gameTime.TotalGameTime.TotalSeconds - holdTime, MAX_ARROW_HOLD);
            // linear interp: y = 500 + (x - 0)(1300 - 500)/(MAX_HOLD-0) x = elapsedTime
            float speed =
                MIN_ARROW_INIT_SPEED + (MAX_ARROW_INIT_SPEED - MIN_ARROW_INIT_SPEED) /
                                       MAX_ARROW_HOLD *
                                       (float)elapsedTime;

            Vector2 initialVelocity = speed * character.Direction;
            arrow.Physics.LinearVelocity += initialVelocity;
        }

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime)
        {
            Fire(character, world, gameTime, holdTime);
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<World>();

                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(64),
                    PhysicsConstants.PixelsToMeters(32),
                    1,
                    _initialPosition);
                Physics.BodyType = BodyType.Dynamic;
                Physics.IsBullet = true;
                Physics.UserData = this;

                initialized = true;
            }
        }
    }
}
