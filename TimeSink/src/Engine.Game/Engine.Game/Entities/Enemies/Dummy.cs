﻿using System;
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

namespace Engine.Game.Entities.Enemies
{
    public class Dummy : Entity, IHaveHealth
    {
        const float DUMMY_MASS = 100f;
        const string DUMMY_TEXTURE = "Textures/Enemies/Dummy";

        private GravityPhysics physics;
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
            physics = new GravityPhysics(position, DUMMY_MASS)
            {
                GravityEnabled = false
            };
            dots = new List<DamageOverTimeEffect>();
        }

        public override ICollisionGeometry CollisionGeometry
        {
            get
            {
                return new CollisionRectangle(
                    new Rectangle(
                        (int)physics.Position.X,
                        (int)physics.Position.Y,
                        64, 128
                    ));
            }
        }

        public override IPhysicsParticle PhysicsController
        {
            get { return physics; }
        }

        public override IRendering Rendering
        {
            get
            {
                var tint = Math.Min(100, 2.55f * health);
                return new TintedRendering(
                  DUMMY_TEXTURE,
                  physics.Position,
                  0,
                  Vector2.One,
                  new Color(255f, tint, tint, 255f));//Math.Max(2.55f * health, 155)));
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(WorldGeometry world, CollisionInfo info)
        {
            // Handle whether collision should disable gravity
            if (info.MinimumTranslationVector.Y > 0)
            {
                physics.GravityEnabled = false;
                physics.Velocity = new Vector2(physics.Velocity.X, Math.Min(0, physics.Velocity.Y));
            }
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Arrow arrow, CollisionInfo info)
        {
            health -= 25;
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Dart dart, CollisionInfo info)
        {
            RegisterDot(dart.dot);
        }

        public override void Update(GameTime time, EngineGame world)
        {
            foreach (DamageOverTimeEffect dot in dots)
            {
                if (dot.Active)
                    health -= dot.Tick(time);
            }
            RemoveInactiveDots();
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
    }
}
