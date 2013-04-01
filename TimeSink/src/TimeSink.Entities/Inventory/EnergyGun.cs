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
using TimeSink.Entities.Enemies;

namespace TimeSink.Entities.Inventory
{
    [SerializableEntity("16b8d25a-25f1-4b0b-acae-c60114aade0e")]
    public class EnergyGun : Entity, IWeapon
    {
        private static readonly string BULLET_TEXTURE = "";
        private static readonly int BULLET_SPEED = 3000;
        private static readonly Guid GUID = new Guid("16b8d25a-25f1-4b0b-acae-c60114aade0e");
        private const int RADIUS = 15;
        private const int TIME_BETWEEN_SHOTS = 100;
        private int timeSinceLastShot = 0;

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
            get { return new List<Fixture>(); }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() { };
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
            timeSinceLastShot += time.ElapsedGameTime.Milliseconds;
            base.OnUpdate(time, world);
        }

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            Fire(character, world, gameTime, holdTime, charged);
        }

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            if (timeSinceLastShot >= TIME_BETWEEN_SHOTS)
            {
                timeSinceLastShot = 0;
                EnergyBullet bullet = new EnergyBullet(
                    new Vector2(character.Physics.Position.X,// + UserControlledCharacter.X_OFFSET,
                                character.Physics.Position.Y + UserControlledCharacter.Y_OFFSET),
                                20, 20,
                                PhysicsConstants.PixelsToMeters(BULLET_SPEED * character.Direction));

                world.LevelManager.RegisterEntity(bullet);
            }
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
            get { return "Textures\\giroux"; }
        }
    }
}
