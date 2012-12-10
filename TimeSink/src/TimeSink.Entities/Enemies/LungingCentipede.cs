using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("849abec2-71f5-4c37-aa71-42d0c161d881")]
    public class LungingCentipede : NormalCentipede
    {
        private bool midLunge;
        private bool initialized;
        private bool waitForWorldContact;

        private UserControlledCharacter character;

        public override Guid Id
        {
            get
            {
                return new Guid("849abec2-71f5-4c37-aa71-42d0c161d881");
            }
            set
            {
                base.Id = value;
            }
        }

        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);

            if (force || !initialized)
            {
                var sensor = FixtureFactory.AttachCircle(
                    PhysicsConstants.PixelsToMeters(200),
                    0,
                    Physics);
                sensor.IsSensor = true;
                sensor.RegisterOnCollidedListener<UserControlledCharacter>(DetectedPlayer);

                Physics.RegisterOnCollidedListener<WorldGeometry2>(HitWorld);
                Physics.RegisterOnSeparatedListener<WorldGeometry2>(LeftWorld);

                initialized = true;
            }
        }

        void LeftWorld(Fixture wheel, WorldGeometry2 w, Fixture wFix)
        {
            if (midLunge && !waitForWorldContact)
            {
                waitForWorldContact = true;
            }
        }

        bool HitWorld(Fixture wheel, WorldGeometry2 w, Fixture wFix, Contact c)
        {
            if (!wheel.IsSensor && midLunge && waitForWorldContact)
            {
                midLunge = false;
                waitForWorldContact = false;

                var force = character.Position - Physics.Position;
                
                if (force.X > 0 && WheelSpeed < 0 || force.X < 0 && WheelSpeed > 0)
                    WheelSpeed *= -1;
            }
            return c.Enabled;
        }

        bool DetectedPlayer(Fixture sensor, UserControlledCharacter ch, Fixture f, Contact c)
        {
            if (!midLunge)
            {
                character = ch;
                midLunge = true;
                Physics.IgnoreGravity = false;

                var force = character.Position - Physics.Position;
                Physics.ApplyForce((force - Vector2.UnitY * 2) * 400);

                if (force.X > 0 && WheelSpeed < 0 || force.X < 0 && WheelSpeed > 0)
                    WheelSpeed *= -1;
            }
            return c.Enabled;
        }
    }
}
