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
    [SerializableEntity("ec4e9acd-6deb-46f4-bfc7-4793b6c39ed9")]
    [EditorEnabled]
    public class SwitchableWall : Wall, ISwitchable
    {
        const string EDITOR_NAME = "Switchable Wall";
        const string TEXTURE = "Textures/Objects/energy_wall";

        private static readonly Guid guid = new Guid("ec4e9acd-6deb-46f4-bfc7-4793b6c39ed9");
        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        public SwitchableWall()
            : this(75, 300)
        {
        }
        public SwitchableWall(int width, int height)
        {
            Width = width;
            Height = height;
            Enabled = true;
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("Enabled")]
        public bool Enabled { get; set; }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
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
                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = false;
                Physics.UserData = this;

                Physics.CollisionCategories = Category.Cat2 | Category.Cat1;
                Physics.CollidesWith = Category.Cat2 | Category.Cat3;

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override IRendering Preview
        {
            get
            {
                if (Enabled)
                    return new BasicRendering(TEXTURE)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache)
                    };
                else
                    return new NullRendering();
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override bool OnCollidedWith(Fixture f1, Inventory.Projectile proj, Fixture f2, Contact contact)
        {
            return base.OnCollidedWith(f1, proj, f2, contact);
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() 
                { 
                    new BasicRendering(TEXTURE)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache)
                    } 
                };
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }

        public void OnSwitch()
        {
            Enabled = !Enabled;
            Physics.Enabled = Enabled;

        }
    }
}
