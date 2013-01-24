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
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Engine.Defaults;

namespace TimeSink.Entities.Objects
{
    [SerializableEntity("e46a6ea6-a61d-40a3-ab2b-57399014dee5")]
    [EditorEnabled]
    public class TutorialBreakBridge : Entity
    {
        const string TEXTURE = "Textures/Objects/ice bridge";
        const string EDITOR_NAME = "Break Bridge Item";
        private static readonly Guid guid = new Guid("e46a6ea6-a61d-40a3-ab2b-57399014dee5");

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override Guid Id { get { return guid; } set { } }


        public TutorialBreakBridge()
            : this(512, 64)
        {
        }

        public TutorialBreakBridge(int width, int height)
        {
            Width = width;
            Height = height;
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

                var rect = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters,
                    1.4f,
                    Vector2.Zero,
                    Physics);

                Physics.BodyType = BodyType.Static;
                Physics.IgnoreGravity = true;
                Physics.IsSensor = false;

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);

                initialized = true;
            }
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            Physics.IgnoreGravity = false;
            Physics.BodyType = BodyType.Dynamic;
            return true;
        }
        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {

        }

        public override Engine.Core.Rendering.IRendering Preview
        {
            get { return new SizedRendering(TEXTURE, PhysicsConstants.MetersToPixels(Position), 0, Width, Height); }
        }

        public override List<FarseerPhysics.Dynamics.Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override Engine.Core.Rendering.IRendering Rendering
        {
            get { return Preview; }
        }
    }
}
