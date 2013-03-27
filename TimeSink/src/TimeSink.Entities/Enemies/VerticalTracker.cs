using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Autofac;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("a0a2ba1a-0692-4f49-adb5-1f333e462649")]
    public class VerticalTracker : Enemy
    {
        private const string TEXTURE = "Textures/giroux";
        private float DEPTH = -50f;
        private float shotTimer = 0f;
        private float bulletTimer = 0f;
        private int shotsFired = 0;
        private bool waitingToShoot = false;

        public bool WaitingToShoot { get { return waitingToShoot; } }
        public bool NeedToJump { get; set; }
        public bool NeedToDescend { get; set; }


        private UserControlledCharacter target;

        private static readonly Guid guid = new Guid("a0a2ba1a-0692-4f49-adb5-1f333e462649");

        public VerticalTracker()
            : this(Vector2.Zero)
        {
        }


        public VerticalTracker(Vector2 position)
            :base(position)
        {
        }

        public override Guid Id { get { return guid; } set{} }

        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);
            var width = PhysicsConstants.PixelsToMeters(Width);
            var height = PhysicsConstants.PixelsToMeters(Height);
            var world = engineRegistrations.Resolve<PhysicsManager>().World;

            FixtureFactory.AttachRectangle(width, height, .1f, Vector2.Zero, Physics);

            Physics.BodyType = BodyType.Dynamic;
            Physics.IsSensor = false;
            Physics.UserData = this;
            Physics.Mass = 2f;
            Physics.Friction = 0.01f;


            
                    
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() 
                {
                    new BasicRendering(TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position)

                }
                };
            }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(TEXTURE)

                {
                    Position = PhysicsConstants.MetersToPixels(Position)

                };
            }
        }

        public void Jump()
        {
            Physics.CollidesWith = Category.All;
            Physics.ApplyForce(new Vector2(0, -1600f));
        }

        public void Descend()
        {
            Physics.CollidesWith = ~Category.Cat31;
            Physics.ApplyForce(new Vector2(0, 200f));
        }

        public void Shoot(EngineGame world)
        {
            var bullet = new SmallBullet(Position, 20, 20, new Vector2(80, 0));
            world.LevelManager.RegisterEntity(bullet);
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            var timeTick = time.ElapsedGameTime.Milliseconds;

            bulletTimer += timeTick;
            if (waitingToShoot)
            {
                shotTimer += timeTick;
            }

            if (shotTimer >= 2000)
            {
                waitingToShoot = false;
                shotTimer = 0f;
            }

            if (!waitingToShoot)
            {
                if (bulletTimer >= 200)
                {
                    Shoot(world);
                    bulletTimer = 0;
                    shotsFired = (shotsFired + 1) % 3;
                }
            }

            if (shotsFired == 2)
            {
                waitingToShoot = true;
                if (NeedToDescend)
                {
                    Descend();
                    NeedToDescend = false;
                }
                if (NeedToJump)
                {
                    Jump();
                    NeedToJump = false;
                }
            }
        }
    }
}
