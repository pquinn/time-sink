using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Collisions;

namespace TimeSink.Entities.Weapons
{
    [EditorEnabled]
    public class Dart : Entity, IWeapon
    {
        const string DART_TEXTURE_NAME = "Textures/Weapons/Dart";
        const string EDITOR_NAME = "Dart";

        public GravityPhysics physics { get; private set; }
        public DamageOverTimeEffect dot { get; private set; }

        const float DART_SPEED = 30;

        private Vector2 _initialPosition;

        public Body Physics { get; private set; }

        public Dart() 
            : this(Vector2.Zero)
        {
        }

        public Dart(Vector2 position)
        {
            //Physics = new GravityPhysics(position, DART_MASS)
            //{
            //    GravityEnabled = true
            //};

            _initialPosition = position;
            dot = new DamageOverTimeEffect(4, 100);
        }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList;
            }
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override string EditorPreview
        {
            get
            {
                return DART_TEXTURE_NAME;
            }
        }

        [EditableField("Position")]
        public Vector2 Position
        {
            get { return physics.Position; }
            set { physics.Position = value; }
        }

        public override IRendering Rendering
        {
            get
            {
                return Dead 
                    ? null 
                    : new BasicRendering(
                        DART_TEXTURE_NAME,
                        PhysicsConstants.MetersToPixels(Physics.Position),
                        (float)Math.Atan2(Physics.LinearVelocity.Y, Physics.LinearVelocity.X),
                        Vector2.One
                    );
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Entity entity, Contact info)
        {
            if (!(entity is UserControlledCharacter))
            {
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
                world.RenderManager.UnregisterRenderable(this);
                world.CollisionManager.UnregisterCollideable(this);
                Physics.Dispose();
            }
            else
            {
                Physics.ApplyForce(new Vector2(0, -.1f));
            }
        }

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime)
        {
            Dart dart = new Dart(
                            new Vector2(character.Physics.Position.X + UserControlledCharacter.X_OFFSET,
                                        character.Physics.Position.Y + UserControlledCharacter.Y_OFFSET));
            world.Entities.Add(dart);
            world.RenderManager.RegisterRenderable(dart);
            world.PhysicsManager.RegisterPhysicsBody(dart);
            world.CollisionManager.RegisterCollideable(dart);

            character.InHold = false;
            Vector2 initialVelocity = character.Direction * DART_SPEED;
            dart.Physics.LinearVelocity += initialVelocity;
        }

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime)
        {
            Fire(character, world, gameTime, holdTime);
        }

        public override void InitializePhysics(World world)
        {
            Physics = BodyFactory.CreateRectangle(
                world,
                PhysicsConstants.PixelsToMeters(16),
                PhysicsConstants.PixelsToMeters(8),
                1,
                _initialPosition);
            Physics.BodyType = BodyType.Dynamic;
            Physics.IsBullet = true;
            Physics.IsSensor = true;
            Physics.UserData = this;
        }
    }
}
