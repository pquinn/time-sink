using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics.Joints;
using TimeSink.Engine.Core;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("fc8cddb4-e1ef-4b83-bc22-5b6460103524")]
    public class TutorialMonster : Enemy
    {
        const float MASS = 100f;
        const string EDITOR_NAME = "Tutorial Monster";

        const float DEPTH = -50f;

        private float lastShotTime;

        private static readonly Guid GUID = new Guid("fc8cddb4-e1ef-4b83-bc22-5b6460103524");

        public TutorialMonster()
            : this(Vector2.Zero, Vector2.Zero)
        {
        }

        public TutorialMonster(Vector2 position, Vector2 direction)
            : base(position)
        {
            DirectionFacing = direction;
        }

        [EditableField("Direction Facing")]
        [SerializableField]
        public Vector2 DirectionFacing { get; set; }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public Body Wheel { get; set; }
        public RevoluteJoint RevJoint { get; set; }
        public bool IsShooting { get; set; }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            if (IsShooting)
            {
                lastShotTime += time.ElapsedGameTime.Milliseconds;
                if (lastShotTime > 5000)
                {
                    Shoot(world);
                    lastShotTime = 0;
                }
            }
        }
            
        public void StartChase()
        {
            //RevJoint.MotorSpeed = 10;
            //RevJoint.MotorTorque = RevJoint.MaxMotorTorque;
        }

        public void EndChase()
        {
            //RevJoint.MotorSpeed = 0;
            //RevJoint.MotorTorque = RevJoint.MaxMotorTorque = 200;
            //RevJoint.LimitEnabled = false;

            IsShooting = true;
        }

        private void Shoot(EngineGame world)
        {
            var largeBullet = new LargeBullet(
                Position + PhysicsConstants.PixelsToMeters(new Vector2(Width / 2, -Height / 4)), 
                200, 30, new Vector2(30, 0));
            world.LevelManager.RegisterEntity(largeBullet);            
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                // Get variables
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = GetTexture(textureCache);
                Width = texture.Width;
                Height = texture.Height;

                var width = PhysicsConstants.PixelsToMeters(Width);
                var height = PhysicsConstants.PixelsToMeters(Height);

                // Create a wheel for motion.
                Wheel = BodyFactory.CreateCircle(
                    world,
                    width / 2,
                    1,
                   Position + new Vector2(0, height / 2));
                Wheel.BodyType = BodyType.Dynamic;
                Wheel.Friction = 10.0f;
                Wheel.Mass = 100;

                // Create a rectangular body for hit detection.
                Physics = BodyFactory.CreateRectangle(
                    world,
                    width,
                    height,
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;
                Physics.Mass = 0f;
                Physics.CollidesWith = Category.Cat1;
                Physics.CollisionCategories = Category.Cat3;

                // Fix the wheel to the body
                RevJoint = JointFactory.CreateRevoluteJoint(world, Physics, Wheel, Vector2.Zero);
                RevJoint.MotorEnabled = true;
                RevJoint.MaxMotorTorque = 1000;
                
                // Register hit detection callbacks.
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }
    }
}
