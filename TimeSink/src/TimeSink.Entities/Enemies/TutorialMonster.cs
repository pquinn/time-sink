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

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("fc8cddb4-e1ef-4b83-bc22-5b6460103524")]
    public class TutorialMonster : Enemy
    {
        const float MASS = 100f;
        const string EDITOR_NAME = "Tutorial Monster";

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

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override void OnUpdate(GameTime time, Engine.Core.EngineGame world)
        {
            base.OnUpdate(time, world);


        }

        private bool initialized;
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

                // Create a wheel for motion.
                Wheel = BodyFactory.CreateCircle(
                    world,
                    Width / 2,
                    1,
                    Position + new Vector2(0, Height / 2));
                Wheel.BodyType = BodyType.Dynamic;
                Wheel.Friction = 10.0f;

                // Create a rectangular body for hit detection.
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;

                // Fix the wheel to the body
                RevJoint = JointFactory.CreateRevoluteJoint(world, Physics, Wheel, Vector2.Zero);
                RevJoint.MotorEnabled = true;
                RevJoint.MaxMotorTorque = 10;
                
                // Register hit detection callbacks.
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat1;

                initialized = true;
            }
        }
    }
}
