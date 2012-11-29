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
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("7d61f455-3da6-4b9d-ad7a-5f4c21e79527")]
    public class Enemy : Entity, IHaveHealth
    {
        #region fields

        private static readonly Guid GUID = new Guid("7d61f455-3da6-4b9d-ad7a-5f4c21e79527");

        const float DUMMY_MASS = 100f;
        const string DUMMY_TEXTURE = "Textures/Enemies/Dummy";
        const string EDITOR_NAME = "Enemy";

        private static int textureHeight;
        private static int textureWidth;

        private List<DamageOverTimeEffect> dots;
        protected float health;

        protected Vector2 _initialPosition;

        #endregion

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

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [EditableField("Health")]
        [SerializableField]
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

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
            c.TakeDamage(25);
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
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                world.LevelManager.CollisionManager.UnregisterCollideable(this);
            }
        }

        private void RemoveInactiveDots()
        {
            dots.RemoveAll(x => x.Finished);
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            var texture = textureCache.LoadResource(DUMMY_TEXTURE);
        }

        public void RegisterDot(DamageOverTimeEffect dot)
        {
            if (!dot.Active)
            {
                dots.Add(dot);
                dot.Active = true;
            }
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<World>();
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = GetTexture(textureCache);
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(texture.Width),
                    PhysicsConstants.PixelsToMeters(texture.Height),
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

                initialized = true;
            }
        }

        protected virtual Texture2D GetTexture(IResourceCache<Texture2D> textureCache)
        {
            return textureCache.GetResource(DUMMY_TEXTURE);
        }
    }
}
