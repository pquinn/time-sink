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

namespace TimeSink.Entities
{
    [EditorEnabled]
    public class NonPlayerCharacter : Entity
    {
        const string EDITOR_NAME = "NPC";
        const string NPC_TEXTURE = "Textures/Enemies/Dummy";

        protected int textureHeight;
        protected int textureWidth;

        protected Vector2 _initialPosition;

        public Body Physics { get; protected set; }

        public NonPlayerCharacter() : this(Vector2.Zero) { }

        public NonPlayerCharacter(Vector2 position)
        {
            _initialPosition = position;
        }

        [EditableField("Position")]
        public Vector2 Position
        {
            get { return Physics.Position; }
            set { Physics.Position = value; }
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            throw new NotImplementedException();
        }

        public override void Load(EngineGame engineGame)
        {
            var texture = engineGame.TextureCache.GetResource(NPC_TEXTURE);
            textureWidth = texture.Width;
            textureHeight = texture.Height;
        }

        public override void InitializePhysics(World world)
        {
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

        public override string EditorPreview
        {
            get
            {
                return NPC_TEXTURE;
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
                return new BasicRendering(
                  NPC_TEXTURE,
                  PhysicsConstants.MetersToPixels(Physics.Position),
                  0f,
                  Vector2.One);
            }
        }
    }
}
