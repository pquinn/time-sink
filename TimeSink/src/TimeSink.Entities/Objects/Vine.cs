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

        private Func<float, float> PatrolFunction { get; set; }
        private bool first;
        private float tZero;

        public float Rotation { get; set; }

        public Vector2 Size { get; set; }

        protected Vector2 _initialPosition;

        public Vine() : this(Vector2.Zero, 1f) { }

        public Vine(Vector2 position) : this(position, 1f) { }

        public Vine(Vector2 position, float timeSpan)
        {
            _initialPosition = position;
            scale = .5f;
            Rotation = 0f;

            float startRotation = -MathHelper.PiOver4;
            float endRotation = MathHelper.PiOver4;

            PatrolFunction = delegate(float time)
            {
                float currentStep = time % timeSpan;
                float newRotation = startRotation;
                if (currentStep >= 0 && currentStep < (timeSpan / 2))
                {
                    var stepAmt = currentStep / timeSpan * 2;
                    newRotation = startRotation + (stepAmt * (endRotation - startRotation));
                }
                else
                {
                    newRotation = endRotation + ((currentStep - timeSpan / 2) / timeSpan * 2 * (startRotation - endRotation));
                }
                return newRotation;
            };
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
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var texture = engineRegistrations.Resolve<IResourceCache<Texture2D>>().GetResource(VINE_TEXTURE);
                var world = engineRegistrations.Resolve<World>();
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters((int)(texture.Width * scale)),
                    PhysicsConstants.PixelsToMeters((int)(texture.Height * scale)),
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

                initialized = true;
            }
        }

        public override IRendering Rendering
        {
            get 
            {
                return new BasicRendering(
                    VINE_TEXTURE,
                    PhysicsConstants.MetersToPixels(Physics.Position),
                    Rotation,
                    new Vector2(scale, scale));
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override void Update(GameTime time, EngineGame world)
        {
            //interpolate the rotation like a line
            base.Update(time, world);

            if (first)
            {
                tZero = (float)time.ElapsedGameTime.TotalSeconds;
                first = false;
            }

            Rotation = PatrolFunction.Invoke((float)time.TotalGameTime.TotalSeconds - tZero);
            Physics.Rotation = Rotation;

            //need to have rotation translated
        }
    }
}
