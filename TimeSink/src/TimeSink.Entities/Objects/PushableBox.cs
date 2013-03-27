using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.States;
using Autofac;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Objects
{
    [SerializableEntity("7c66912c-6c14-4714-ab5d-27778ee8c063")]
    [EditorEnabled]
    public class PushableBox : Entity
    {
        private const string EDITOR_NAME = "Pushable Box";

        private static Guid guid = new Guid("7c66912c-6c14-4714-ab5d-27778ee8c063");
        private bool initialized = false;

        public PushableBox()
            : this(200, 200)
        { }

        public PushableBox(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override Guid Id
        {
            get
            {
                return guid;
            }
            set
            {
            }
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        public override Engine.Core.Rendering.IRendering Preview
        {
            get
            {
                return new BasicRendering("Textures/Objects/Crate")
                    {

                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, "Textures/Objects/Crate", TextureCache)
                    };
            }
        }

        public override List<FarseerPhysics.Dynamics.Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override List<Engine.Core.Rendering.IRendering> Renderings
        {
            get 
            {
                var list = new List<IRendering>();
                list.Add(Preview);
                return list;
            }
        }

        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {

            if (!initialized || force)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;

                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                Physics = BodyFactory.CreateRectangle(
                          world,
                          spriteWidthMeters, spriteHeightMeters,
                          1.4f, Position);
                Physics.BodyType = BodyType.Dynamic;
                Physics.IsSensor = false;
                Physics.UserData = this;
                Physics.Mass = 75f;
                Physics.Friction = 0.01f;
                            
                Physics.CollisionCategories = Category.All;
                Physics.CollidesWith = Category.All;

                initialized = true;
            }
            base.InitializePhysics(force, engineRegistrations);
        }
    }
}
