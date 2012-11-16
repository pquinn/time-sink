using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Game.Entities.Weapons;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    public class NormalCentipede : Enemy, IHaveHealth
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Goomba";
        const string EDITOR_NAME = "Normal Centipede";

        private List<DamageOverTimeEffect> dots;
        public NormalCentipede()
            : this(Vector2.Zero)
        {
        }

        public NormalCentipede(Vector2 position)
            : base(position)
        {
            health = 150;
            physics = new GravityPhysics(position, CENTIPEDE_MASS)
            {
                GravityEnabled = true
            };
        }

        private Vector2 _initialPosition;

        [EditableField("Position")]
        public Vector2 Position
        {
            get { return physics.Position; }
            set { physics.Position = value; }
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override string EditorPreview
        {
            get
            {
                return CENTIPEDE_TEXTURE;
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList;
            }
        }

        public Body Physics { get; private set; }

        public override IRendering Rendering
        {
            get
            {
                var tint = Math.Min(100, 2.55f * health);
                return new TintedRendering(
                  CENTIPEDE_TEXTURE,

                  PhysicsConstants.MetersToPixels(Physics.Position),
                  0,
                  Vector2.One,
                  new Color(255f, tint, tint, 255f));
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(UserControlledCharacter c, Contact info)
        {
            c.Health -= 25;
        }


        public override void Update(GameTime time, EngineGame world)
        {
            base.Update(time, world);
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(CENTIPEDE_TEXTURE);
        }

        public override void InitializePhysics(World world)
        {
            Physics = BodyFactory.CreateRectangle(
                world,
                PhysicsConstants.PixelsToMeters(32),
                PhysicsConstants.PixelsToMeters(32),
                1,
                _initialPosition);
            Physics.BodyType = BodyType.Dynamic;
            Physics.FixedRotation = true;
            Physics.UserData = this;

            var fix = Physics.FixtureList[0];
            fix.CollisionCategories = Category.Cat3;
            fix.CollidesWith = Category.Cat1 | Category.Cat2;

            //var hitsensor = fix.Clone(Physics);
            //hitsensor.IsSensor = true;
            //hitsensor.CollidesWith = Category.Cat2;
            //hitsensor.CollisionCategories = Category.Cat2;
        }
    }
}
