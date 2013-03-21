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
using TimeSink.Entities.Enemies;
using Microsoft.Xna.Framework;
using TimeSink.Entities.Utils;

namespace TimeSink.Entities.Objects
{
    [SerializableEntity("a849cb69-ed2c-48ad-bdee-35015063d331")]
    [EditorEnabled]
    public class BreakableWall : Wall, ISwitchable
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

                Physics.CollisionCategories = Category.Cat2 | Category.Cat1;
                Physics.CollidesWith = Category.Cat2 | Category.Cat3;

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public bool BulletHit()
        {
            remainingHits--;
            if (remainingHits <= 0)
            {
                this.DestroyPhysics();
            }
            return true;
        }

        public bool DestroyWall()
        {
            this.remainingHits = 0;
            this.DestroyPhysics();
            return true;
        }

        public override IRendering Preview
        {
            get
            {
                if (remainingHits == 3)
                    return new BasicRendering(TEXTURE1)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE1, TextureCache)
                    };
                else if (remainingHits == 2)
                    return new BasicRendering(TEXTURE2)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE2, TextureCache)
                    };
                else if (remainingHits == 1)
                    return new BasicRendering(TEXTURE3)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE3, TextureCache)
                    };
                else
                    return new NullRendering();
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override List<IRendering> Renderings
        {
            get { return new List<IRendering>() { Preview }; }
        }
        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }

        public void OnSwitch()
        {
            DestroyPhysics();
        }
    }
}
