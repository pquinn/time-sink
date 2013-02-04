using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Entities.Inventory;
using TimeSink.Engine.Core.Editor;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using TimeSink.Engine.Core.States;
using Engine.Defaults;

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
        protected float health;

        public HashSet<DamageOverTimeEffect> Dots { get; set; }

        #endregion

        public Enemy()
            : this(Vector2.Zero)
        {
        }

        public Enemy(Vector2 position)
        {
            health = 100;
            Position = position;

            Dots = new HashSet<DamageOverTimeEffect>();
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
                return new BasicRendering(DUMMY_TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    TintColor = new Color(255f, tint, tint, 255f)
                };
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        protected bool OnCollidedWith(Fixture f, Arrow arrow, Fixture af, Contact info)
        {
            health -= 100;
            return true;
        }

        protected bool OnCollidedWith(Fixture f, Dart dart, Fixture df, Contact info)
        {
            RegisterDot(dart.dot);
            return true;
        }

        protected bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            c.TakeDamage(25);
            return true;
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (health <= 0)
            {
                Dead = true;
            }

            RemoveInactiveDots();

            foreach (DamageOverTimeEffect dot in Dots)
            {
                if (dot.Active)
                    health -= dot.Tick(time);
            }

            if (Dead)
            {
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                world.LevelManager.PhysicsManager.UnregisterPhysicsBody(this);
            }
        }

        private void RemoveInactiveDots()
        {
            Dots.RemoveWhere(x => x.Finished);
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
                Dots.Add(dot);
                dot.Active = true;
            }
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = GetTexture(textureCache);
                Width = texture.Width;
                Height = texture.Height;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;

                Physics.RegisterOnCollidedListener<Arrow>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<Dart>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat1;

                var hitsensor = fix.Clone(Physics);
                hitsensor.IsSensor = true;
                hitsensor.CollisionCategories = Category.Cat2;
                hitsensor.CollidesWith = Category.Cat2;

                initialized = true;

                Physics.RegisterOnCollidedListener<Arrow>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<Dart>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        protected virtual Texture2D GetTexture(IResourceCache<Texture2D> textureCache)
        {
            return textureCache.GetResource(DUMMY_TEXTURE);
        }
    }
}
