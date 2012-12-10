﻿using System;
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
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Autofac;
using TimeSink.Engine.Core.Caching;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Collisions;

namespace TimeSink.Entities
{
    [EditorEnabled]
    [SerializableEntity("c31fb7ad-f9de-4ca3-a091-521583c6c6bf")]
    public class MovingPlatform : Entity
    {
        const string WORLD_TEXTURE_NAME = "Textures/Tiles/MovingPlatform";
        const string EDITOR_NAME = "Moving Geometry";

        private static readonly Guid GUID = new Guid("c31fb7ad-f9de-4ca3-a091-521583c6c6bf");

        protected int textureHeight;
        protected int textureWidth;

        private Func<float, Vector2> PatrolFunction { get; set; }
        private int direction;
        private bool first;
        private float tZero;

        public MovingPlatform() : this(Vector2.Zero, Vector2.Zero, 0, 0, 0) { }

        //define discrete start and end for platforms
        public MovingPlatform(Vector2 startPosition, Vector2 endPosition, float timeSpan, int width, int height)
            : base()
        {
            Position = startPosition;
            StartPosition = startPosition;
            EndPosition = endPosition;
            TimeSpan = timeSpan;
            Width = width > 0 ? width : 50;
            Height = height > 0 ? width : 50;
            direction = 1;
            first = true;
            PatrolFunction = delegate(float time)
            {
                float currentStep = time % TimeSpan;
                Vector2 newPosition = new Vector2();
                if (currentStep >= 0 && currentStep < (TimeSpan / 2f))
                {
                    var stepAmt = currentStep / TimeSpan * 2;
                    newPosition = StartPosition + (stepAmt * (EndPosition - StartPosition));
                }
                else
                {
                    newPosition = EndPosition + ((currentStep - TimeSpan / 2) / TimeSpan * 2 * (StartPosition - EndPosition));
                }
                return newPosition;
            };
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("Start Position")]
        public Vector2 StartPosition { get; set; }

        [SerializableField]
        [EditableField("End Position")]
        public Vector2 EndPosition { get; set; }

        [SerializableField]
        [EditableField("Time Span")]
        public float TimeSpan { get; set; }

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
                return new SizedRendering(WORLD_TEXTURE_NAME, PhysicsConstants.MetersToPixels(Physics.Position), 0, Width, Height);
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

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.UserData = this;
                Physics.BodyType = BodyType.Kinematic;
                Physics.Friction = 5f;
                Physics.IgnoreGravity = true;
                Physics.CollidesWith = Category.All | ~Category.Cat1;
                Physics.CollisionCategories = Category.Cat1;

                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat1;
                fix.CollidesWith = Category.All | ~Category.Cat1;

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }

        public override void Load(IComponentContext container)
        {
            var texture = container.Resolve<IResourceCache<Texture2D>>().GetResource(WORLD_TEXTURE_NAME);
            textureWidth = texture.Width;
            textureHeight = texture.Height;
        }
    }
}
