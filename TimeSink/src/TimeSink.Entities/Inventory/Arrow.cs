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
using Microsoft.Xna.Framework.Audio;
using Engine.Defaults;

namespace TimeSink.Entities.Inventory
{
    [SerializableEntity("16b8d25a-25f1-4b0b-acae-c60114aade0e")]
    public class Arrow : Entity, IWeapon
    {
        const float ARROW_MASS = .02f;
        const string ARROW_TEXTURE_NAME = "Textures/Weapons/Arrow";
        const string FLAME_TEXTURE = "Textures/Weapons/ArrowFlames";
        const string EDITOR_NAME = "Arrow";
        const string ARROW_IMPACT_SOUND = "Audio/Sounds/ArrowImpact";

        const float MAX_ARROW_HOLD = 1;
        const float MIN_ARROW_INIT_SPEED = 1000;
        const float MAX_ARROW_INIT_SPEED = 2000;

        const float DEPTH = 150;

        public bool OnFire { get; set; }

        private static readonly Guid GUID = new Guid("16b8d25a-25f1-4b0b-acae-c60114aade0e");
        private SoundEffect arrowImpact;

        public Arrow()
            : this(Vector2.Zero)
        {
        }

        public Arrow(Vector2 position)
        {
            Position = position;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public string Texture
        {
            get { return ARROW_TEXTURE_NAME; }
        }
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
                return new BasicRendering(!OnFire ? ARROW_TEXTURE_NAME : FLAME_TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Physics.Position),
                    Rotation = (float)Math.Atan2(Physics.LinearVelocity.Y, Physics.LinearVelocity.X),
                    DepthWithinLayer = .1f
                };
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public bool OnCollidedWith(Fixture f, Entity entity, Fixture eFix, Contact info)
        {
            if (info.Enabled && !(entity is UserControlledCharacter || entity is Trigger || entity is Ladder || entity is Torch))
            {
                Dead = true;
                arrowImpact.Play();
            }
            return info.Enabled;
        }
        public bool OnCollidedWith(Fixture f, Torch torch, Fixture fb, Contact info)
        {
            this.OnFire = true;
            return true;
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            textureCache.LoadResource(ARROW_TEXTURE_NAME);
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            if (Dead)
            {
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                world.LevelManager.PhysicsManager.UnregisterPhysicsBody(this);
            }
            else
                Physics.Rotation = (float)Math.Atan2(Physics.LinearVelocity.Y, Physics.LinearVelocity.X);
        }

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            Arrow arrow = new Arrow(
                new Vector2(character.Physics.Position.X,// + UserControlledCharacter.X_OFFSET,
                            character.Physics.Position.Y + UserControlledCharacter.Y_OFFSET));

            world.LevelManager.RegisterEntity(arrow);

            character.InHold = false;

            var elapsedTime = Math.Min(gameTime.TotalGameTime.TotalSeconds - holdTime, MAX_ARROW_HOLD);
            // linear interp: y = 500 + (x - 0)(1300 - 500)/(MAX_HOLD-0) x = elapsedTime
            float speed =
                MIN_ARROW_INIT_SPEED + (MAX_ARROW_INIT_SPEED - MIN_ARROW_INIT_SPEED) /
                                       MAX_ARROW_HOLD *
                                       (float)elapsedTime;

            Vector2 initialVelocity = PhysicsConstants.PixelsToMeters(speed * character.Direction);
            arrow.Physics.LinearVelocity += initialVelocity;
        }

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            Fire(character, world, gameTime, holdTime, charged);
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var soundCache = engineRegistrations.Resolve<IResourceCache<SoundEffect>>();
                var world = engineRegistrations.Resolve<PhysicsManager>().World;

                arrowImpact = soundCache.LoadResource(ARROW_IMPACT_SOUND);

                Width = 64;
                Height = 32;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.BodyType = BodyType.Dynamic;
                Physics.IsBullet = true;
                Physics.UserData = this;
                Physics.IsSensor = true;
                Physics.Mass = ARROW_MASS;
                Physics.CollidesWith = Category.All | ~Category.Cat31;

                Physics.RegisterOnCollidedListener<Entity>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<Torch>(OnCollidedWith);

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }
    }
}
