using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.States;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Entities
{
    [EditorEnabled]
    [SerializableEntity("57eb5766-5ce2-4694-ad4b-e019d4817985")]
    public class NonPlayerCharacter : Entity
    {
        const string EDITOR_NAME = "NPC";
        const string NPC_TEXTURE = "Textures/Enemies/Dummy";

        private static readonly Guid GUID = new Guid("57eb5766-5ce2-4694-ad4b-e019d4817985");

        protected int textureHeight;
        protected int textureWidth;

        protected Vector2 _initialPosition;

        public NonPlayerCharacter() : this(Vector2.Zero) { }

        public NonPlayerCharacter(Vector2 position)
        {
            _initialPosition = position;
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
            throw new NotImplementedException();
        }

        public override void Load(IComponentContext container)
        {
            var texture = container.Resolve<IResourceCache<Texture2D>>().GetResource(NPC_TEXTURE);
            textureWidth = texture.Width;
            textureHeight = texture.Height;
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
            Physics.BodyType = BodyType.Dynamic;
            Physics.UserData = this;

            var fix = Physics.FixtureList[0];
            fix.CollisionCategories = Category.Cat3;
            fix.CollidesWith = Category.Cat1;

            var hitsensor = fix.Clone(Physics);
            hitsensor.IsSensor = true;
            hitsensor.CollisionCategories = Category.Cat2;
            hitsensor.CollidesWith = Category.Cat2;
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
                return new BasicRendering(
                  NPC_TEXTURE,
                  PhysicsConstants.MetersToPixels(Physics.Position),
                  0f,
                  Vector2.One);
            }
        }
    }
}
