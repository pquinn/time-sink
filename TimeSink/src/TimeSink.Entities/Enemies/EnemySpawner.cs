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
        [EditableField("Spawn Interval (ms)")]
        [SerializableField]
        public float SpawnInterval { get; set; }

        [EditableField("Spawn Amount")]
        [SerializableField]
        public int SpawnAmount { get; set; }

        [EditableField("Initial Interval Offset")]
        [SerializableField]
        public float SpawnOffset { get; set; }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }
        
        float counter;
        float inBetween = 800f;
        float betweenTime;

        public EnemySpawner() : 
            this(10000f, 0, int.MaxValue, 50, 50) { }

        public EnemySpawner(float interval, float offset, int max, int width, int height)
        {
            SpawnInterval = interval;
            MaxSpawn = max;
            Height = height;
            Width = width;
            counter += offset % interval;
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

                var hitBox = FixtureFactory.AttachCircle(
                    PhysicsConstants.PixelsToMeters(50),
                    1,
                    Physics);

                Physics.IsSensor = true;

                hitBox.RegisterOnCollidedListener<Arrow>(collidedArrow);
                //hitBox.RegisterOnCollidedListener<Dart>(collidedDart);
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

        [EditableField("Max Spawn Amt")]
        [SerializableField]
        public int MaxSpawn { get; set; }

        void separatedEnemy(Fixture f1, T e, Fixture eF)
        {
            if (justSpawned.Contains(e))
                justSpawned.Remove(e);
        }

        bool collidedEnemy(Fixture f1, T e, Fixture eF, Contact c)
        {
            if (!justSpawned.Contains(e))
                e.Dead = true;
            return c.Enabled;
        }

        bool collidedArrow(Fixture f1, Arrow e, Fixture f2, Contact c)
        {
            if (e.OnFire)
                Dead = true;

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

        private bool canSpawn;
        private int batchCount;
        private bool firstTick;
        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            if (firstTick)
            {
                firstTick = false;
                counter = SpawnOffset;
            }

            counter += (float)time.ElapsedGameTime.TotalMilliseconds;
            betweenTime += time.ElapsedGameTime.Milliseconds;

            spawned.RemoveWhere(x => x.Dead);
            justSpawned.RemoveWhere(x => x.Dead);

            if (counter > SpawnInterval && spawned.Count < MaxSpawn)
            {
                canSpawn = true;
                counter = 0;
            }

            if (canSpawn && betweenTime > inBetween)
            {
                var enemy = SpawnEnemy(time, world);
                spawned.Add(enemy);
                justSpawned.Add(enemy);

                world.LevelManager.RegisterEntity(enemy);
                
                betweenTime = 0;
                batchCount += 1;

                if (batchCount == SpawnAmount)
                {
                    batchCount = 0;
                    canSpawn = false;
                }
            }
        }

        protected abstract T SpawnEnemy(GameTime time, EngineGame world);
    }
}
