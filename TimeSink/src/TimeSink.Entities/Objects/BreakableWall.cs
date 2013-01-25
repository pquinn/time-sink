using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
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

namespace TimeSink.Entities.Objects
{
    [SerializableEntity("a849cb69-ed2c-48ad-bdee-35015063d331")]
    [EditorEnabled]
    public class BreakableWall : Entity
    {
        const string EDITOR_NAME = "Breakable Wall";
        const string TEXTURE1 = "Textures/Objects/Ice Wall";
        const string TEXTURE2 = "Textures/Objects/Ice Wall2";
        const string TEXTURE3 = "Textures/Objects/Ice Wall3";
        const string TEXTURE4 = "Textures/Objects/Ice Wall4";
        private int remainingHits = 3;

        private static readonly Guid guid = new Guid("a849cb69-ed2c-48ad-bdee-35015063d331");
        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        public BreakableWall()
            : this(75, 300)
        {
        }
        public BreakableWall(int width, int height)
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
            if (!initialized || force)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height / 2);

                Physics = BodyFactory.CreateRectangle(
                    world,
                    spriteWidthMeters, spriteHeightMeters,
                    1.4f, Position);
                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = false;
                Physics.UserData = this;

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

                initialized = true;
            }
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            remainingHits--;
            if (remainingHits <= 0)
            {
                this.DestroyPhysics();
            }
            return true;
        }

        public override Engine.Core.Rendering.IRendering Preview
        {
            get
            {
                if (remainingHits == 3)
                    return new SizedRendering(TEXTURE1, PhysicsConstants.MetersToPixels(Position), 0, Width, Height);
                else if (remainingHits == 2)
                    return new SizedRendering(TEXTURE2, PhysicsConstants.MetersToPixels(Position), 0, Width, Height);
                else if (remainingHits == 1)
                    return new SizedRendering(TEXTURE3, PhysicsConstants.MetersToPixels(Position), 0, Width, Height);
                else
                    return new SizedRendering(TEXTURE4, PhysicsConstants.MetersToPixels(Position), 0, Width, Height);
            }
        }

        public override List<FarseerPhysics.Dynamics.Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override Engine.Core.Rendering.IRendering Rendering
        {
            get { return Preview; }
        }
        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }
    }
}
