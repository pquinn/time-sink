using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Collisions;

namespace TimeSink.Entities
{
    public class OneWayPlatform
    {
        private Shape collisionShape;

        private Dictionary<Fixture, bool> separatedDict = new Dictionary<Fixture, bool>();

        public OneWayPlatform(Fixture f)
        {
            var sensor = f.Clone(f.Body);
            sensor.IsSensor = true;
            //sensor.OnSeparation += new OnSeparationEventHandler((f1, f2) => Console.WriteLine());
            sensor.OnSeparation += new OnSeparationEventHandler(OnSeparation);
            f.UserData = this;
        }

        void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            Fixture f;
            if (fixtureB.Body.UserData is UserControlledCharacter)
                f = fixtureB;
            else if (fixtureA.Body.UserData is UserControlledCharacter)
                f = fixtureA;
            else
                return;

            separatedDict[f] = true;
        }

        [OnCollidedWith.Overload]
        public static bool OnCollidedWith(UserControlledCharacter player, WorldGeometry world, Contact contact)
        {
            var fixtureA = contact.FixtureA;
            var fixtureB = contact.FixtureB;

            Fixture f;
            if (fixtureB.Body.UserData is WorldGeometry)
                f = fixtureB;
            else if (fixtureA.Body.UserData is WorldGeometry)
                f = fixtureA;
            else
                return true;

            if (f.UserData is OneWayPlatform)
                return (f.UserData as OneWayPlatform).OnCollision(fixtureA == f ? fixtureB : fixtureA);
            return true;
        }

        private bool OnCollision(Fixture f)
        {
            bool result = false;

            bool separatedAFrame = false;
            bool found = separatedDict.TryGetValue(f, out separatedAFrame);
            separatedAFrame = !found || separatedAFrame;

            if (separatedAFrame)
            {
                if ((f.Body.UserData as UserControlledCharacter).Physics.LinearVelocity.Y > .01f)
                    result = true;
            }

            separatedDict[f] = false;

            return result;
        }
    }
}
