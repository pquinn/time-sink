using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core;
using FarseerPhysics.Dynamics.Contacts;
using Engine.Defaults;
using TimeSink.Entities.Objects;
using TimeSink.Engine.Core.Collisions;
using Autofac;
using FarseerPhysics.Factories;
using TimeSink.Entities.Triggers;

namespace TimeSink.Entities.Inventory
{
    [SerializableEntity("16b8d25a-25f1-4b0b-acae-c60114aade0e")]
    public class EnergyGun : Projectile, IWeapon
    {
        private static readonly string BULLET_TEXTURE = "";
        private static readonly int BULLET_SPEED = 3;
        private static readonly Guid GUID = new Guid("16b8d25a-25f1-4b0b-acae-c60114aade0e");
        private const int RADIUS = 15;

        public EnergyGun()
            : this(Vector2.Zero)
        {
        }

        public EnergyGun(Vector2 position)
        {
            Position = position;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>()
                {
                    new BasicRendering(BULLET_TEXTURE)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        DepthWithinLayer = .1f
                    }
                };
            }
        }

        public bool OnCollidedWith(Fixture f, Entity entity, Fixture eFix, Contact info)
        {
            if (info.Enabled && !(entity is UserControlledCharacter || entity is Trigger || entity is Ladder || entity is Torch || entity is TutorialTrigger || entity is NonPlayerCharacter))
            {
                Dead = true;
            }
            else
            {
                return false;
            }
            return info.Enabled;
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            if (Dead)
            {
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                world.LevelManager.PhysicsManager.UnregisterPhysicsBody(this);
            }
        }

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            Fire(character, world, gameTime, holdTime, charged);
        }

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            EnergyGun bullet = new EnergyGun(
                new Vector2(character.Physics.Position.X,// + UserControlledCharacter.X_OFFSET,
                            character.Physics.Position.Y + UserControlledCharacter.Y_OFFSET));

            world.LevelManager.RegisterEntity(bullet);

            bullet.Physics.LinearVelocity = PhysicsConstants.PixelsToMeters(BULLET_SPEED * character.Direction);
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;

                Width = 64;
                Height = 32;
                Physics = BodyFactory.CreateCircle(
                    world,
                    RADIUS,
                    1,
                    Position,
                    this);

                Physics.BodyType = BodyType.Dynamic;
                Physics.IsBullet = true;
                Physics.IsSensor = true;
                Physics.CollidesWith = ~Category.Cat31;

                Physics.RegisterOnCollidedListener<Entity>(OnCollidedWith);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }

        public override string EditorName
        {
            get { throw new NotImplementedException(); }
        }

        public override IRendering Preview
        {
            get { throw new NotImplementedException(); }
        }

        public string Texture
        {
            get { throw new NotImplementedException(); }
        }
    }
}
