using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Common;

namespace TimeSink.Entities.Objects
{
    [SerializableEntity("e00b5cb8-cbeb-482d-8614-d8d2cd201608")]
    [EditorEnabled]
   public class BreakableBridge : Entity
    {

        const string EDITOR_NAME = "Breakable Bridge";
        const string TEXTURE = "Textures/Objects/ice bridge";
        private static readonly Guid Guid = new Guid("e00b5cb8-cbeb-482d-8614-d8d2cd201608");
        private BreakableBody piece;

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public override Guid Id { get { return Guid; } set { } }

        public BreakableBridge()
            : this(512, 64, Vector2.Zero)
        {
        }
        public BreakableBridge(int width, int height, Vector2 position)
        {
            Width = width;
            Height = height;
            Position = position;
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                Physics = BodyFactory.CreateBody(world, Position, this);

                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

            /*    var rect = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters,
                    1.4f,
                    Vector2.Zero,
                    Physics);*/
                

                IList<Vector2> vertices = new List<Vector2>();
                vertices.Add(Vector2.Zero);
                vertices.Add(new Vector2(0, spriteHeightMeters));
                vertices.Add(new Vector2(spriteWidthMeters, 0));
                vertices.Add(new Vector2(spriteWidthMeters, spriteHeightMeters));
                vertices.Add(Vector2.Zero);
                Vertices v = new Vertices(vertices);

                var b = BodyFactory.CreateBreakableBody(
                    world,
                    v,
                    1.4f);

                
              //  var revJoint = JointFactory.CreateRevoluteJoint(rect, rect2, rect2.Position);

                Physics.BodyType = BodyType.Dynamic;
                Physics.IsSensor = false;
                Physics.IgnoreGravity = true;

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
               // Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        bool OnCollidedWith(Fixture f, UserControlledCharacter character, Fixture cFix, Contact info)
        {
            return true;
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache)
                };
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override Engine.Core.Rendering.IRendering Rendering
        {
            get { return Preview; }
        }
    }
}
