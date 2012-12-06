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
using FarseerPhysics.Collision;
using FarseerPhysics.Common;

namespace TimeSink.Entities
{
    public class OneWayPlatform
    {
        private Dictionary<Fixture, bool> separatedDict = new Dictionary<Fixture, bool>();

        public OneWayPlatform(Fixture f)
        {
            //var sensor = f.Clone(f.Body);
            //sensor.IsSensor = true;
            //sensor.OnSeparation += new OnSeparationEventHandler(OnSeparation);
            f.UserData = this;
        }

        //void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        //{
        //    Fixture f;
        //    if (fixtureB.Body.UserData is Entity)
        //        f = fixtureB;
        //    else if (fixtureA.Body.UserData is Entity)
        //        f = fixtureA;
        //    else
        //        return;

        //    if (!f.IsSensor)
        //        separatedDict[f] = true;
        //}

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
            {
                if (fixtureA == f)
                    return (f.UserData as OneWayPlatform).OnCollision(player, fixtureB, fixtureA, contact);
                else
                    return (f.UserData as OneWayPlatform).OnCollision(player, fixtureA, fixtureB, contact);
            }
            return true;
        }

        private bool OnCollision(Entity e, Fixture entityFixture, Fixture platformFixture, Contact contact)
        {
            //bool result = false;

            //bool separatedAFrame = false;
            //bool found = separatedDict.TryGetValue(entityFixture, out separatedAFrame);
            //separatedAFrame = !found || separatedAFrame;

            //if (separatedAFrame)
            //{
            //    if (e.Physics.LinearVelocity.Y > .1f)
            //        result = true;
            //}

            //foreach (var fix in e.CollisionGeometry.Where(x => !x.IsSensor))
            //    separatedDict[fix] = false;

            //return result;

            if (e.PreviousPosition != null && e.Physics.LinearVelocity.Y > .1f)
            {
                Vector2 normal;
                FixedArray2<Vector2> points;
                contact.GetWorldManifold(out normal, out points);

                var offset = (e.PreviousPosition ?? Vector2.Zero) - e.Position;

                var edgeShape = platformFixture.Shape as EdgeShape;
                var platformVector = edgeShape.Vertex1.X < edgeShape.Vertex2.X
                    ? edgeShape.Vertex2 - edgeShape.Vertex1
                    : edgeShape.Vertex1 - edgeShape.Vertex2;

                for (int i = 0; i < contact.Manifold.PointCount; i++)
                {
                    var entityVector = points[i] + offset - edgeShape.Vertex1;

                    var cross = Vector3.Cross(
                        new Vector3(entityVector, 0), 
                        new Vector3(platformVector, 0));

                    if (cross.Z < 0)
                        return true;
                }
            }

            return false;
        }
    }
}
