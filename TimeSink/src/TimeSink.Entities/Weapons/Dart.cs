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
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Objects;

namespace TimeSink.Entities.Weapons
{
    [SerializableEntity("158e2984-34ce-4c1f-93ef-fbf81c5fed1f")]
    public class Dart : Entity, IWeapon
    {
        const string DART_TEXTURE_NAME = "Textures/Weapons/Dart";
        const string EDITOR_NAME = "Dart";

        private static readonly Guid GUID = new Guid("158e2984-34ce-4c1f-93ef-fbf81c5fed1f");

        public GravityPhysics physics { get; private set; }
        public DamageOverTimeEffect dot { get; private set; }

        const float DART_SPEED = 2000;

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

            Position = position;
            dot = new DamageOverTimeEffect(4, 100);
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

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
        
        public override IRendering Preview
        {
            get { return Rendering; }
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
        public bool OnCollidedWith(Entity entity, Contact info)
        {
            if (!(entity is UserControlledCharacter) && !(entity is Ladder))
            {
                Dead = true;
            }
            return true;
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            textureCache.LoadResource(DART_TEXTURE_NAME);
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            if (Dead)
            {
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                world.LevelManager.CollisionManager.UnregisterCollideable(this);
               // Physics.Dispose();
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
            world.LevelManager.RegisterEntity(dart);

            character.InHold = false;
            Vector2 initialVelocity = PhysicsConstants.PixelsToMeters(character.Direction * DART_SPEED);
            dart.Physics.LinearVelocity += initialVelocity;
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

                Width = 16;
                Height = 8;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.BodyType = BodyType.Dynamic;
                Physics.IsBullet = true;
                Physics.IsSensor = true;
                Physics.UserData = this;

                initialized = true;
            }
        }
    }
}
