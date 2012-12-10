using System;
using System.Collections.Generic;
using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Weapons;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("bb7f91f9-af92-41cc-a985-bd1e85066403")]
    public class FlyingCentipede : Enemy
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Flying Centipede/Flying01"; //temporary
        const string EDITOR_NAME = "Flying Centipede";

        private static readonly Guid GUID = new Guid("bb7f91f9-af92-41cc-a985-bd1e85066403");

        private static int textureHeight;
        private static int textureWidth;

        private bool first;
        private float tZero;

        public FlyingCentipede()
            : this(Vector2.Zero, Vector2.Zero, 0f)
        {
            first = true;
        }

        public FlyingCentipede(Vector2 startPosition, Vector2 endPosition, float timeSpan) : base()
        {
            health = 150;
            Position = startPosition;
            StartPosition = startPosition;
            EndPosition = endPosition;
            TimeSpan = timeSpan;
            first = true;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        [EditableField("Start Position")]
        public Vector2 StartPosition { get; set; }

        [SerializableField]
        [EditableField("End Position")]
        public Vector2 EndPosition { get; set; }

        [SerializableField]
        [EditableField("Time Span")]
        public float TimeSpan { get; set; }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override IRendering Rendering
        {
            get
            {
                var tint = Math.Min(100, 2.55f * health);
                return new TintedRendering(
                  CENTIPEDE_TEXTURE,
                  PhysicsConstants.MetersToPixels(Position),
                  0,
                  Vector2.One,
                  new Color(255f, tint, tint, 255f));
            }
        }

        public override IRendering Preview
        {
            get
            {
                return Rendering;
            }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            if (first)
            {
                tZero = (float)time.TotalGameTime.TotalSeconds;
                first = false;
            }

            float currentStep = ((float)time.TotalGameTime.TotalSeconds - tZero) % TimeSpan;
            var stepAmt = currentStep / TimeSpan;
            var dir = Math.Sin(stepAmt * 2 * Math.PI);
            var offset = EndPosition - StartPosition;
            var len = offset.Length();
            offset.Normalize();
            if (dir > 0)
                Physics.LinearVelocity = Vector2.Multiply(offset, (float)(len / (TimeSpan / 2)));
            else if (dir < 0)
                Physics.LinearVelocity = -Vector2.Multiply(offset, (float)(len / (TimeSpan / 2)));
            else
                Physics.LinearVelocity = Vector2.Zero;
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            var texture = textureCache.LoadResource(CENTIPEDE_TEXTURE);
            textureWidth = texture.Width;
            textureHeight = texture.Height; 
        }

        protected override Texture2D GetTexture(IResourceCache<Texture2D> textureCache)
        {
            return textureCache.GetResource(CENTIPEDE_TEXTURE);
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = GetTexture(textureCache);
                Width = (int)(texture.Width * .65);
                Height = (int)(texture.Height * .5);
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;
                Physics.IsSensor = true;
                Physics.CollisionCategories = Category.Cat3 | Category.Cat2;
                Physics.CollidesWith = Category.Cat1 | Category.Cat2;
                Physics.IgnoreGravity = true;

                Physics.RegisterOnCollidedListener<Arrow>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<Dart>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }
    }
}
