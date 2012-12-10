using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Weapons;

namespace TimeSink.Entities.Enemies
{
    public abstract class EnemySpawner<T> : Enemy where T : Enemy
    {
        [SerializableField]
        public float SpawnInterval;
        
        float counter;

        public EnemySpawner() : 
            this(1000f, int.MaxValue) { }

        public EnemySpawner(float interval, int max)
        {
            SpawnInterval = interval;
            MaxSpawn = max;
        }

        public override void Load(IComponentContext engineRegistrations)
        {

        }

        private bool initialized;

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                Physics = BodyFactory.CreateBody(world, Position, this);
                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = true;

                var hitBox = FixtureFactory.AttachCircle(
                    PhysicsConstants.PixelsToMeters(50),
                    1,
                    Physics);

                hitBox.RegisterOnCollidedListener<Arrow>(collidedArrow);
                hitBox.RegisterOnCollidedListener<Dart>(collidedDart);
                hitBox.RegisterOnCollidedListener<T>(collidedEnemy);
                hitBox.RegisterOnSeparatedListener<T>(separatedEnemy);

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized)
                return;

            Physics.Dispose();
            initialized = false;
        }

        HashSet<T> justSpawned = new HashSet<T>();
        HashSet<T> spawned = new HashSet<T>();

        [SerializableField]
        public int MaxSpawn;

        void separatedEnemy(Fixture f1, T e, Fixture eF)
        {
            if (justSpawned.Contains(e))
                justSpawned.Remove(e);
            else
                e.Dead = true;
        }

        bool collidedEnemy(Fixture f1, T e, Fixture eF, Contact c)
        {
            return c.Enabled;
        }

        bool collidedArrow(Fixture f1, Arrow e, Fixture f2, Contact c)
        {
            this.Health -= 25;
            return c.Enabled;
        }

        bool collidedDart(Fixture f1, Dart e, Fixture f2, Contact c)
        {
            this.RegisterDot(e.dot);
            return c.Enabled;
        }

        public override Guid Id
        {
            get
            {
                return new Guid("349aaec2-aa55-4c37-aa71-42d0c1616885");
            }
            set { }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            counter += time.ElapsedGameTime.Milliseconds;

            spawned.RemoveWhere(x => x.Dead);
            justSpawned.RemoveWhere(x => x.Dead);

            if (counter > SpawnInterval && spawned.Count < MaxSpawn)
            {
                var enemy = SpawnEnemy(time, world);
                spawned.Add(enemy);
                justSpawned.Add(enemy);

                world.LevelManager.PhysicsManager.RegisterPhysicsBody(enemy);
                world.LevelManager.RenderManager.RegisterRenderable(enemy);
                
                counter = 0;
            }
        }

        protected abstract T SpawnEnemy(GameTime time, EngineGame world);
    }
}
