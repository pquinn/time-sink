using System.Collections.Generic;
using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Rendering;
using System;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities.Enemies
{

    public class EnemyKiller<T> : Entity where T : Enemy
    {        
        private static readonly Guid guid = new Guid("a4759c34-9b58-476a-8ff5-682ead129543");
        private static readonly string editorName = "Enemy Killer";

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
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

                var hitBox = FixtureFactory.AttachRectangle(
                    PhysicsConstants.PixelsToMeters(50),
                    PhysicsConstants.PixelsToMeters(50),
                    1,
                    Vector2.Zero,
                    Physics);

                Physics.IsSensor = true;

                hitBox.RegisterOnCollidedListener<T>(CollidedEnemy);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override void DestroyPhysics()
        {
            if (!initialized)
                return;

            Physics.Dispose();
            initialized = false;
        }

        protected virtual bool CollidedEnemy(Fixture f1, T e, Fixture f2, Contact c)
        {
            e.Dead = true;

            return true;
        }

        public override string EditorName
        {
            get { return editorName; }
        }

        public override System.Guid Id
        {
            get
            {
                return guid;
            }
            set
            {
            }
        }

        public override IRendering Preview
        {
            get { return new NullRendering(); }
        }

        public override IRendering Rendering
        {
            get { return new NullRendering(); }
        }
    }
}
