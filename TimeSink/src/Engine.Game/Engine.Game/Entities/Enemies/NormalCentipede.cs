using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Game.Entities;
using TimeSink.Engine.Game.Entities.Weapons;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;

namespace Engine.Game.Entities.Enemies
{
    class NormalCentipede : Entity, IHaveHealth
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Goomba";

        private List<DamageOverTimeEffect> dots;


        private Vector2 _initialPosition;

        private float health;
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList;
            }
        }

        public NormalCentipede(Vector2 position)
        {
            health = 100;
            _initialPosition = position;
            dots = new List<DamageOverTimeEffect>();
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
            throw new NotImplementedException();
        }

        //[OnCollidedWith.Overload]
        //public void OnCollidedWith(WorldGeometry world, CollisionInfo info)
        //{
        //    // Handle whether collision should disable gravity
        //    if (info.MinimumTranslationVector.Y > 0)
        //    {
        //        Physics.GravityEnabled = false;
        //        Physics.Velocity = new Vector2(Physics.Velocity.X, Math.Min(0, Physics.Velocity.Y));
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

        [OnCollidedWith.Overload]
        public void OnCollidedWith(UserControlledCharacter c, Contact info)
        {
            c.Health -= 25;
        }

        public override void Update(GameTime time, EngineGame world)
        {
            if (health <= 0)
            {
                Console.WriteLine("goomba dead");
                Dead = true;
            }

            foreach (DamageOverTimeEffect dot in dots)
            {
                if (dot.Active)
                    health -= dot.Tick(time);
            }
            RemoveInactiveDots();


            if (Dead)
            {
                world.RenderManager.UnregisterRenderable(this);
                world.CollisionManager.UnregisterCollideable(this);
            }
        }

        private void RemoveInactiveDots()
        {
            // there has to be a better way to do this.........
            List<DamageOverTimeEffect> newDots = new List<DamageOverTimeEffect>();
            foreach (DamageOverTimeEffect dot in dots)
            {
                if (!dot.Finished)
                    newDots.Add(dot);
            }
            dots = newDots;
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(CENTIPEDE_TEXTURE);
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
