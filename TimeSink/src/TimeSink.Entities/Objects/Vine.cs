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

namespace TimeSink.Entities.Objects
{
    [EditorEnabled]
    public class Vine : Entity
    {

        const string EDITOR_NAME = "Vine";
        const string VINE_TEXTURE = "Textures/Objects/Vine";

        protected int textureHeight;
        protected int textureWidth;
        protected float scale;

        public Body Physics { get; protected set; }

        protected Vector2 _initialPosition;

        public Vine() : this(Vector2.Zero) { }

        public Vine(Vector2 position)
        {
            _initialPosition = position;
            scale = .25f;
        }

        [EditableField("Position")]
        public Vector2 Position
        {
            get { return Physics.Position; }
            set { Physics.Position = value; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            throw new NotImplementedException();
        }

        public override void Load(EngineGame engineGame)
        {
            var texture = engineGame.TextureCache.GetResource(VINE_TEXTURE);
            textureWidth = (int)(texture.Width * scale);
            textureHeight = (int)(texture.Height * scale);
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
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
