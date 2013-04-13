using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using Autofac;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities.Inventory
{
    [SerializableEntity("16b7d25a-15f1-4b0b-acaf-c70124acda0e")]
    public class TimeGrenade : Projectile, IWeapon
    {
        const string EDITOR_NAME = "TimeGrenade";

        const string GRENADE_TEXTURE_NAME = "Textures/Weapons/TimeGrenade";
        const string EXPLOSION_TEXTURE_NAME = "Textures/Weapons/TimeGrenade_Blast";

        const float MAX_GRENADE_HOLD = 1;
        const float MIN_GRENADE_INIT_SPEED = 1000;
        const float MAX_GRENADE_INIT_SPEED = 2000;

        const double DEFAULT_FUSE_TIME = 4000;
        const double DEFAULT_LINGER_TIME = 10000;

        private Vector2 initialVelocity;

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        private static readonly Guid GUID = new Guid("16b7d25a-15f1-4b0b-acaf-c70124acda0e");

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        private double fuseTime;

        public override IRendering Preview
        {
            get { return new NullRendering(); }
        }

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
                    new BasicRendering(!exploded ? GRENADE_TEXTURE_NAME : EXPLOSION_TEXTURE_NAME)
                    {
                        Position = PhysicsConstants.MetersToPixels(Physics.Position),
                        Rotation = Physics.Rotation,
                        DepthWithinLayer = .1f
                    }
                }; 
            }
        }

        public TimeGrenade(Vector2 pos, Vector2 vel, double fuseTime_ms, double lingerTime_ms)
        {
            Position = pos;
            initialVelocity = vel;
            fuseTime = fuseTime_ms;
            lingerTime = lingerTime_ms;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                Physics = BodyFactory.CreateCircle(
                    world,
                    PhysicsConstants.PixelsToMeters(7),
                    1,
                    Position,
                    this);

                Physics.BodyType = BodyType.Dynamic;
                Physics.CollidesWith = Category.All;
                Physics.LinearVelocity = initialVelocity;
            }
        }

        private bool exploded;
        private double lingerTime;

        private TimeScaleCircle blast;

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (!exploded)
            {
                fuseTime -= time.ElapsedGameTime.TotalMilliseconds;
                if (fuseTime <= 0)
                {
                    exploded = true;
                    blast = new TimeScaleCircle()
                    {
                        Center = Position,
                        Radius = PhysicsConstants.PixelsToMeters(100),
                        TimeScale = .3f
                    };
                    world.LevelManager.Level.TimeScaleCircles.Add(blast);
                }
            }
            else
            {
                lingerTime -= time.ElapsedGameTime.TotalMilliseconds;
                if (lingerTime <= 0)
                {
                    world.LevelManager.RenderManager.UnregisterRenderable(this);
                    world.LevelManager.PhysicsManager.UnregisterPhysicsBody(this);
                    world.LevelManager.Level.TimeScaleCircles.Remove(blast);
                    Dead = true;
                }
            }
        }

        #region IWeapon Members

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            character.InHold = false;

            var elapsedTime = Math.Min(gameTime.TotalGameTime.TotalSeconds - holdTime, MAX_GRENADE_HOLD);
            // linear interp: y = 500 + (x - 0)(1300 - 500)/(MAX_HOLD-0) x = elapsedTime
            float speed =
                MIN_GRENADE_INIT_SPEED + (MAX_GRENADE_INIT_SPEED - MIN_GRENADE_INIT_SPEED) /
                                       MAX_GRENADE_HOLD *
                                       (float)elapsedTime;

            Vector2 initialVelocity = PhysicsConstants.PixelsToMeters(speed * character.Direction);
            if (character.Direction.Y == 0)
            {
                var rotation = (float)Math.PI / 32;
                rotation *= character.Direction.X > 0
                    ? -1
                    : 1;
                initialVelocity = Vector2.Transform(initialVelocity, Matrix.CreateRotationZ(rotation));
            }
            
            TimeGrenade grenade = new TimeGrenade(
                new Vector2(character.Physics.Position.X,// + UserControlledCharacter.X_OFFSET,
                            character.Physics.Position.Y + UserControlledCharacter.Y_OFFSET),
                initialVelocity,
                DEFAULT_FUSE_TIME,
                DEFAULT_LINGER_TIME);

            world.LevelManager.RegisterEntity(grenade);
        }

        #endregion

        #region IInventoryItem Members

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            Fire(character, world, gameTime, holdTime, charged);
        }

        #endregion

        #region IMenuItem Members

        public string Texture
        {
            get { throw new NotImplementedException(); }
        }

        #endregion


        public void ChargeInitiated(UserControlledCharacter character, GameTime gameTime)
        {
        }

        public void ChargeReleased(UserControlledCharacter character, GameTime gameTime)
        {
        }
    }
}
