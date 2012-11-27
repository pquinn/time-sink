using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.States;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Entities.Objects
{
    [EditorEnabled]
    [SerializableEntity("b425aa27-bc56-4953-aa4c-be089fdc29c8")]
    public class Vine : Entity
    {
        const string EDITOR_NAME = "Vine";
        const string VINE_TEXTURE = "Textures/Objects/Vine";

        private static readonly Guid GUID = new Guid("b425aa27-bc56-4953-aa4c-be089fdc29c8");

        protected int textureHeight;
        protected int textureWidth;
        protected float scale;

        public Vector2 Size { get; set; }

        protected Vector2 _initialPosition;

        public Vine() : this(Vector2.Zero) { }

        public Vine(Vector2 position)
        {
            _initialPosition = position;
            scale = .25f;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(IComponentContext container)
        {
            var texture = container.Resolve<IResourceCache<Texture2D>>().GetResource(VINE_TEXTURE);
            textureWidth = (int)(texture.Width * scale);
            textureHeight = (int)(texture.Height * scale);
            Size = new Vector2(textureWidth, textureHeight);
        }

        public override void InitializePhysics(IComponentContext engineRegistrations)
        {
            var world = engineRegistrations.Resolve<World>();
            Physics = BodyFactory.CreateRectangle(
                world,
                PhysicsConstants.PixelsToMeters(textureWidth),
                PhysicsConstants.PixelsToMeters(textureHeight),
                1,
                _initialPosition);
            Physics.FixedRotation = true;
            Physics.BodyType = BodyType.Static;
            Physics.UserData = this;

            var fix = Physics.FixtureList[0];
            fix.CollisionCategories = Category.Cat3;
            fix.CollidesWith = Category.Cat1;

            var hitsensor = fix.Clone(Physics);
            hitsensor.IsSensor = true;
            hitsensor.CollisionCategories = Category.Cat2;
            hitsensor.CollidesWith = Category.Cat2;
        }

        public override IRendering Rendering
        {
            get 
            {
                return new BasicRendering(
                    VINE_TEXTURE,
                    PhysicsConstants.MetersToPixels(Physics.Position),
                    0f,
                    new Vector2(scale, scale));
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }
    }
}
