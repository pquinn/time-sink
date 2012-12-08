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
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Objects
{
    [EditorEnabled]
    [SerializableEntity("b425aa27-bc56-4953-aa4c-be089fdc29c8")]
    public class Vine : Entity
    {
        const string EDITOR_NAME = "Vine";
        const string VINE_TEXTURE = "Textures/Objects/Vine";

        private static readonly Guid GUID = new Guid("b425aa27-bc56-4953-aa4c-be089fdc29c8");

        public float TextureHeight { get; set; }
        public float TextureWidth { get; set; }
        protected float scale;

        //private Func<float, float> PatrolFunction { get; set; }
        private bool first;
        private float tZero;

        public Body VineAnchor { get; set; }
        public Body VineEndAffector { get; set; }

        public Vine() : this(Vector2.Zero, 1f) { }

        public Vine(Vector2 position) : this(position, 1f) { }

        public Vine(Vector2 position, float timeSpan)
        {
            Position = position;
            scale = .5f;

            /*
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
             * */
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
                /*
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
                 * */

                var world = engineRegistrations.Resolve<World>();
                var texture = engineRegistrations.Resolve<IResourceCache<Texture2D>>().GetResource(VINE_TEXTURE);

                Width = (int)(texture.Width * scale);
                Height = (int)(texture.Height * scale);
                TextureWidth = PhysicsConstants.PixelsToMeters(Width);
                TextureHeight = PhysicsConstants.PixelsToMeters(Height);

                //anchor point
                Physics = BodyFactory.CreateBody(world, Position, this);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Static;

                VineAnchor = BodyFactory.CreateRectangle(
                    world,
                    TextureWidth,
                    TextureHeight,
                    1,
                    Position);
                VineAnchor.BodyType = BodyType.Dynamic;
                VineAnchor.UserData = this;

                var joint1 = JointFactory.CreateRevoluteJoint(world, Physics, VineAnchor, new Vector2(0, -TextureHeight / 2));

                /*
                VineEndAffector = BodyFactory.CreateRectangle(
                    world,
                    vineWidth,
                    vineWidth,
                    1,
                    _initialPosition + new Vector2(0, vineHeight)
                    );
                VineEndAffector.BodyType = BodyType.Dynamic;

                var joint2 = JointFactory.CreateRevoluteJoint(world, VineAnchor, VineEndAffector, new Vector2(0, 0));
                 * */

                var fix = VineAnchor.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat2;

                var hitsensor = fix.Clone(VineAnchor);
                hitsensor.IsSensor = true;
                hitsensor.CollisionCategories = Category.Cat2;
                hitsensor.CollidesWith = Category.All;

                /*
                var endFix = VineEndAffector.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat1;

                var endHitsensor = fix.Clone(VineEndAffector);
                endHitsensor.IsSensor = true;
                endHitsensor.CollisionCategories = Category.Cat2;
                endHitsensor.CollidesWith = Category.All;
                 * */

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                VineAnchor.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
            }
        }

        bool OnCollidedWith(Fixture f, UserControlledCharacter character, Fixture cFix, Contact info)
        {
            //this don't do shit yet
            return true;
        }

        public override IRendering Rendering
        {
            get 
            {
                return new PivotedRendering(
                    VINE_TEXTURE,
                    PhysicsConstants.MetersToPixels(Physics.Position),
                    //Need to translate rotation
                    VineAnchor == null ? 0 : VineAnchor.Rotation,
                    new Vector2(scale, scale));
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            //interpolate the rotation like a line
            base.OnUpdate(time, world);



            /*
            if (first)
            {
                tZero = (float)time.ElapsedGameTime.TotalSeconds;
                first = false;
            }

            Rotation = PatrolFunction.Invoke((float)time.TotalGameTime.TotalSeconds - tZero);
            Physics.Rotation = Rotation;
            */
        }
    }
}
