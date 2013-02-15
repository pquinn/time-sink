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
        //private Dictionary<Fixture, bool> separatedDict = new Dictionary<Fixture, bool>();
        private HashSet<Fixture> collided = new HashSet<Fixture>();

        public OneWayPlatform(Fixture f)
        {
            f.CollisionCategories = Category.Cat31;

            var sensor = f.Clone(f.Body);
            sensor.IsSensor = true;
            CollisionUtils.RegisterOnSeparatedListener<Entity>(sensor, OnSeparation);

            f.UserData = this;
            CollisionUtils.RegisterOnCollidedListener<Entity>(f, OnCollision);
        }

        void OnSeparation(Fixture platformFixture, Entity entity, Fixture entityFixture)
        {
            if (!entityFixture.IsSensor)
                collided.Remove(entityFixture);
        }

        bool OnCollision(Fixture platformFixture, Entity entity, Fixture entityFixture, Contact contact)
        {
            if (entityFixture.IsSensor)
                return false;

            bool result = false;

            if (!collided.Contains(entityFixture))
            {
                Vector2 normal;
                FixedArray2<Vector2> points;
                contact.GetWorldManifold(out normal, out points);

                //check if contact points are moving into platform
                for (int i = 0; i < contact.Manifold.PointCount; i++)
                {
                    var pointVelPlatform = platformFixture.Body.GetLinearVelocityFromWorldPoint(points[i]);
                    var pointVelEntity = entity.Physics.GetLinearVelocityFromWorldPoint(points[i]);

                    var relativeVel = platformFixture.Body.GetLocalVector(pointVelEntity - pointVelPlatform);

                    if (relativeVel.Y > 1) //if moving down faster than 1 m/s, handle as before
                    {
                        result = true;//point is moving into platform, leave contact solid and exit
                        break;
                    }
                    else if (relativeVel.Y > -1)
                    { 
                        //if moving slower than 1 m/s
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
            }

            collided.Add(entityFixture);

            return result;
        }
    }
}
