using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Entities.Weapons;
using TimeSink.Engine.Core.Editor;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    public class Enemy : Entity, IHaveHealth
    {
        const float DUMMY_MASS = 100f;
        const string DUMMY_TEXTURE = "Textures/Enemies/Dummy";
        const string EDITOR_NAME = "Enemy";

        private List<DamageOverTimeEffect> dots;
        protected float health;        


        public Enemy()
            : this(Vector2.Zero)
        {
        }

        public Enemy(Vector2 position)
        {
            health = 100;
            _initialPosition = position;

            dots = new List<DamageOverTimeEffect>();
        }

        protected Vector2 _initialPosition;

        public Body Physics { get; protected set; }

        protected int textureHeight;
        protected int textureWidth;

        [EditableField("Health")]
        public float Health
        {
            get { return health; }
            set { health = value; }
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

        public override string EditorPreview
        {
            get
            {
                return DUMMY_TEXTURE;
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
                var tint = Math.Min(100, 2.55f * health);
                return new TintedRendering(
                  DUMMY_TEXTURE,
                  PhysicsConstants.MetersToPixels(Physics.Position),
                  0,
                  Vector2.One,
                  new Color(255f, tint, tint, 255f));//Math.Max(2.55f * health, 155)));
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Arrow arrow, Contact info)
        {
            health -= 25;
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Dart dart, Contact info)
        {
            RegisterDot(dart.dot);
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(UserControlledCharacter c, Contact info)
        {
            c.Health -= 25;
        }

        public override void Update(GameTime time, EngineGame world)
        {
            if (health <= 0)
            {
                Console.WriteLine("dummy dead");
                Dead = true;
            }

            RemoveInactiveDots();

            foreach (DamageOverTimeEffect dot in dots)
            {
                if (dot.Active)
                    health -= dot.Tick(time);
            }

            if (Dead)
            {
                world.RenderManager.UnregisterRenderable(this);
                world.CollisionManager.UnregisterCollideable(this);
            }
        }

        private void RemoveInactiveDots()
        {
            dots.RemoveAll(x => x.Finished);
        }

        public override void Load(EngineGame engineGame)
        {
            var texture = engineGame.TextureCache.LoadResource(DUMMY_TEXTURE);
            textureWidth = texture.Width;
            textureHeight = texture.Height;
        }

        public void RegisterDot(DamageOverTimeEffect dot)
        {
            if (!dot.Active)
            {
                dots.Add(dot);
                dot.Active = true;
            }
        }

        public override void InitializePhysics(World world)
        {
            Physics = BodyFactory.CreateRectangle(
                world,
                PhysicsConstants.PixelsToMeters(textureWidth),
                PhysicsConstants.PixelsToMeters(textureHeight),
                1,
                _initialPosition);
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
    }
}
