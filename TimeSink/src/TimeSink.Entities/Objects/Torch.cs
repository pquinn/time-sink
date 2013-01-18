using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.States;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics.Joints;

namespace TimeSink.Entities.Objects
{    
    [EditorEnabled]
    [SerializableEntity("749a40d4-fd31-438d-8d68-9d7a7700ea2d")]
    
    public class Torch : Entity, IInventoryItem
    {
        const string EDITOR_NAME = "Torch";
        const string TEXTURE = "Textures/Objects/TorchFlaming";

        private static readonly Vector2 SrcRectSize = new Vector2(29.5f, 130f);
        private static readonly Guid GUID = new Guid("749a40d4-fd31-438d-8d68-9d7a7700ea2d");

        private WeldJoint j;
        private World w;
        private UserControlledCharacter held;
        private float timer;
        private float interval = 200f;
        private NewAnimationRendering rendering;

        public Torch()
            : this(Vector2.Zero, 10, 75)
        {
        }

        public Torch(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
            rendering =  new NewAnimationRendering(
                  TEXTURE,
                  SrcRectSize,
                  2,
                  Vector2.Zero,
                  0,
                  Vector2.One,
                  Color.White);
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        public override void Load(IComponentContext container)
        {
            var texture = container.Resolve<IResourceCache<Texture2D>>().GetResource(TEXTURE);
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        private bool initialized;
        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                Physics = BodyFactory.CreateBody(world, Position, this);

                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                var rect = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters,
                    1.4f,
                    Vector2.Zero,
                    Physics);

                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.IgnoreGravity = true;
                Physics.IsSensor = true;
                w = world;

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }

        bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            return true;
        }

        void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
        }

        public override IRendering Preview
        {
            get { return new BasicRendering(TEXTURE, PhysicsConstants.MetersToPixels(Position), 0, Vector2.One, new Rectangle(0, 0, (int)SrcRectSize.X, (int)SrcRectSize.Y)); }
        }

        public override List<FarseerPhysics.Dynamics.Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override IRendering Rendering
        {
            get
            {
                return rendering;
            }
        }

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime)
        {
        }

        public string Texture
        {
            get { return TEXTURE; }
        }
        public void WeldToPlayer(UserControlledCharacter c)
        {
            this.Physics.Position = c.Physics.Position;
            this.held = c;
           // j = JointFactory.CreateWeldJoint(Physics, c.Physics, Vector2.Zero);
            //w.AddJoint(j);
        }
        public void PlaceTorch(UserControlledCharacter c, TorchGround ground)
        {

            Physics.Position = new Vector2(c.Physics.Position.X, ground.Physics.Position.Y - 
                                           (PhysicsConstants.PixelsToMeters(Height) / 2));
            held = null;
          //  w.RemoveJoint(j);
        }
        public override void OnUpdate(GameTime time, EngineGame world)
        {
            timer += (float)time.ElapsedGameTime.TotalMilliseconds;
            base.OnUpdate(time, world);
            if (held != null)
            {
                Physics.Position = held.Physics.Position;
            }
            if (timer >= interval)
            {
                rendering.CurrentFrame = (rendering.CurrentFrame + 1) % rendering.NumFrames;
                timer = 0f;
            }

            rendering.Position = PhysicsConstants.MetersToPixels(Physics.Position);

        }
    }
}
