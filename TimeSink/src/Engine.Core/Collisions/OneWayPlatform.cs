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
            f.CollisionCategories = Category.Cat31;

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

            if (!f.IsSensor)
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
            bool result = false;

            bool separatedAFrame = false;
            bool found = separatedDict.TryGetValue(entityFixture, out separatedAFrame);
            separatedAFrame = !found || separatedAFrame;

            if (separatedAFrame)
            {
                Vector2 normal;
                FixedArray2<Vector2> points;
                contact.GetWorldManifold(out normal, out points);

                //check if contact points are moving into platform
                for (int i = 0; i < contact.Manifold.PointCount; i++)
                {
                    var pointVelPlatform = platformFixture.Body.GetLinearVelocityFromWorldPoint(points[i]);
                    var pointVelEntity = e.Physics.GetLinearVelocityFromWorldPoint(points[i]);

                    var relativeVel = platformFixture.Body.GetLocalVector(pointVelEntity - pointVelPlatform);

                    if (relativeVel.Y > 1) //if moving down faster than 1 m/s, handle as before
                    {
                        result = true;//point is moving into platform, leave contact solid and exit
                        break;
                    }
                    else if (relativeVel.Y > -1)
                    { //if moving slower than 1 m/s
                        //borderline case, moving only slightly out of platform
                        var relativePoint = platformFixture.Body.GetLocalPoint(points[i]);
                        float platformFaceY = 0.5f;//front of platform, from fixture definition :(
                        if (relativePoint.Y > platformFaceY - 0.05)
                        {
                            result = true;//contact point is less than 5cm inside front face of platfrom
                            break;
                        }
                    }
                }


                //if (e.PreviousPosition != null && e.Physics.LinearVelocity.Y > .1f)
                //{
                //Vector2 normal;
                //FixedArray2<Vector2> points;
                //contact.GetWorldManifold(out normal, out points);

                //    var offset = (e.PreviousPosition ?? Vector2.Zero) - e.Position;

                //    var edgeShape = platformFixture.Shape as EdgeShape;
                //    var platformVector = edgeShape.Vertex1.X < edgeShape.Vertex2.X
                //        ? edgeShape.Vertex2 - edgeShape.Vertex1
                //        : edgeShape.Vertex1 - edgeShape.Vertex2;

                //    for (int i = 0; i < contact.Manifold.PointCount; i++)
                //    {
                //        var entityVector = points[i] + offset - edgeShape.Vertex1;

                //        var cross = Vector3.Cross(
                //            new Vector3(entityVector, 0),
                //            new Vector3(platformVector, 0));

                //        if (cross.Z > 0)
                //        {
                //            result = true;
                //            break;
                //        }
                //    }
                //}
            }

            
            separatedDict[entityFixture] = false;

            return result;
        }
    }
}
