using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using Microsoft.Xna.Framework;
using Autofac;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Entities.Weapons;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Enemies
{
    public class EnemySpawner : Enemy
    {
        public override void Load(IComponentContext engineRegistrations)
        {
            throw new NotImplementedException();
        }

        public override string EditorName
        {
            get { return "Spawner"; }
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

                initialized = true;
            }
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
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override IRendering Preview
        {
            get { return new NullRendering(); }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override IRendering Rendering
        {
            get { return new NullRendering(); }
        }
    }
}
