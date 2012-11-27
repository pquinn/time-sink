using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Entities
{
    public class MovingPlatform : Entity
    {
        const string WORLD_TEXTURE_NAME = "Textures/giroux";
        const string EDITOR_NAME = "Moving Geometry";

        public Body Physics { get; protected set; }

        protected Vector2 _initialPosition;

        protected int textureHeight;
        protected int textureWidth;

        protected int Width { get; set; }
        protected int Height { get; set; }

        private Func<float, Vector2> PatrolFunction { get; set; }
        private int direction;
        private Vector2 StartPosition { get; set; }
        private Vector2 EndPosition { get; set; }
        private bool first;
        private float tZero;


        public MovingPlatform() : this(Vector2.Zero, Vector2.Zero, 0, 0, 0) { }

        //define discrete start and end for platforms
        public MovingPlatform(Vector2 startPosition, Vector2 endPosition, float timeSpan, int width, int height)
            : base()
        {
            _initialPosition = startPosition;
            StartPosition = startPosition;
            EndPosition = endPosition;
            Width = width;
            Height = height;
            direction = 1;
            first = true;
            PatrolFunction = delegate(float time)
            {
                float currentStep = time % timeSpan;
                Vector2 newPosition = new Vector2();
                if (currentStep >= 0 && currentStep < (timeSpan / 2))
                {
                    var stepAmt = currentStep / timeSpan * 2;
                    newPosition = startPosition + (stepAmt * (endPosition - startPosition));
                }
                else
                {
                    newPosition = endPosition + ((currentStep - timeSpan / 2) / timeSpan * 2 * (startPosition - endPosition));
                }
                return newPosition;
            };
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override string EditorPreview
        {
            get
            {
                return WORLD_TEXTURE_NAME;
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList;
            }
        }

        public override IRendering Rendering
        {
            get
            {
                var tint = Math.Min(100, 2.55f * 100);
                return new TintedRendering(
                  WORLD_TEXTURE_NAME,
                  PhysicsConstants.MetersToPixels(Physics.Position),
                  0,
                  Vector2.One,
                  new Color(255f, tint, tint, 255f));
            }
        }

        public override void Update(GameTime time, EngineGame world)
        {
            base.Update(time, world);

            if (first)
            {
                tZero = (float)time.ElapsedGameTime.TotalSeconds;
                first = false;
            }

            Physics.Position = PatrolFunction.Invoke((float)time.TotalGameTime.TotalSeconds - tZero);
            // should return a vector that represents how much the body moves each tick
            // that way, if it's supposed to reverse, that vector can just be negated
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void InitializePhysics(World world)
        {
            Physics = BodyFactory.CreateRectangle(
                world,
                PhysicsConstants.PixelsToMeters(Width),
                PhysicsConstants.PixelsToMeters(Height),
                1,
                _initialPosition);
            Physics.UserData = this;
            Physics.BodyType = BodyType.Static;
            Physics.Friction = .5f;
            Physics.CollidesWith = Category.All | ~Category.Cat1;
            Physics.CollisionCategories = Category.Cat1;

            var fix = Physics.FixtureList[0];
            fix.CollisionCategories = Category.Cat1;
            fix.CollidesWith = Category.All | ~Category.Cat1;

            //fix.Shape.Density = Single.PositiveInfinity;
        }

        public override void Load(EngineGame engineGame)
        {
            Texture2D texture = engineGame.TextureCache.GetResource(WORLD_TEXTURE_NAME);
            textureWidth = texture.Width;
            textureHeight = texture.Height;
        }
    }
}
