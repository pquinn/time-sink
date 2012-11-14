using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Game.Entities;
using TimeSink.Engine.Game.Entities.Weapons;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace Engine.Game.Entities.Enemies
{
    public class Dummy : Entity, IHaveHealth
    {
        const float DUMMY_MASS = 100f;
        const string DUMMY_TEXTURE = "Textures/Enemies/Dummy";

        private Vector2 _initialPosition;

        public Body Physics { get; private set; }

        private List<DamageOverTimeEffect> dots;

        private float health;
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public Dummy(Vector2 position)
        {
            health = 100;
            _initialPosition = position;
            dots = new List<DamageOverTimeEffect>();
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

        //[OnCollidedWith.Overload]
        //public void OnCollidedWith(WorldGeometry world, CollisionInfo info)
        //{
        //    // Handle whether collision should disable gravity
        //    if (info.MinimumTranslationVector.Y > 0)
        //    {
        //        physics.GravityEnabled = false;
        //        physics.Velocity = new Vector2(physics.Velocity.X, Math.Min(0, physics.Velocity.Y));
        //    }
        //}

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
            engineGame.TextureCache.LoadResource(DUMMY_TEXTURE);
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
                PhysicsConstants.PixelsToMeters(64),
                PhysicsConstants.PixelsToMeters(128),
                1,
                _initialPosition);
            Physics.BodyType = BodyType.Dynamic;
            Physics.UserData = this;
        }
    }
}
