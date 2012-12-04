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
using TimeSink.Engine.Core;

namespace TimeSink.Entities
{
    public class OneWayPlatform
    {
        private Dictionary<Fixture, bool> separatedDict = new Dictionary<Fixture, bool>();

        public OneWayPlatform(Fixture f)
        {
            var sensor = f.Clone(f.Body);
            sensor.IsSensor = true;
            sensor.OnSeparation += new OnSeparationEventHandler(OnSeparation);
            f.UserData = this;
        }

        void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            Fixture f;
            if (fixtureB.Body.UserData is Entity)
                f = fixtureB;
            else if (fixtureA.Body.UserData is Entity)
                f = fixtureA;
            else
                return;

            separatedDict[f] = true;
        }

        [OnCollidedWith.Overload]
        public static bool OnCollidedWith(Entity player, WorldGeometry2 world, Contact contact)
        {
            var fixtureA = contact.FixtureA;
            var fixtureB = contact.FixtureB;

            Fixture f;
            if (fixtureB.Body.UserData is WorldGeometry2)
                f = fixtureB;
            else if (fixtureA.Body.UserData is WorldGeometry2)
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
                if ((f.Body.UserData as Entity).Physics.LinearVelocity.Y > .01f)
                    result = true;
            }

            foreach (var fix in f.Body.FixtureList)
                separatedDict[fix] = false;
            //separatedDict[f] = false;

            return result;
        }
    }
}
