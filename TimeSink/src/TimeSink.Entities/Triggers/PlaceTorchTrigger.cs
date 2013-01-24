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
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Triggers
{

    [EditorEnabled]
    [SerializableEntity("aa70e739-3d5d-454c-bfb1-64b2dc190e3e")]
    public class PlaceTorchTrigger : Entity
    {
        const string EDITOR_NAME = "Place Torch Trigger";
        const string TEXTURE = "Textures/Tiles/Torch_Ground";

        private static readonly Guid GUID = new Guid("aa70e739-3d5d-454c-bfb1-64b2dc190e3e");

        public PlaceTorchTrigger()
            : this(Vector2.Zero, 50, 10)
        {
        }

        public PlaceTorchTrigger(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        public override void HandleKeyboardInput(Microsoft.Xna.Framework.GameTime gameTime, EngineGame world)
        {
        }

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
                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = true;

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
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
            get { return new NullRendering(); }
        }
    }
}
