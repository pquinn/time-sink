using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Engine.Core.Collisions
{
    public class CollisionManager
    {
        public void RegisterCollideable(ICollideable coll)
        {
            foreach (var geo in coll.CollisionGeometry)
            {
                geo.OnCollision += onCollision;
                geo.OnSeparation += onSeparation;
            }
        }

        private static bool onCollision(Fixture f1, Fixture f2, Contact contact)
        {
            return OnCollidedWith.Invoke(
                    f1.Body.UserData as ICollideable,
                    f2.Body.UserData as ICollideable,
                    contact)
                && OnCollidedWith.Invoke(
                    f2.Body.UserData as ICollideable,
                    f1.Body.UserData as ICollideable,
                    contact);
        }

        private static void onSeparation(Fixture f1, Fixture f2)
        {
            OnSeparation.Invoke(
                f1.Body.UserData as ICollideable, f1,
                f2.Body.UserData as ICollideable, f2);
            OnSeparation.Invoke(
                f2.Body.UserData as ICollideable, f2,
                f1.Body.UserData as ICollideable, f1);
        }

        public void UnregisterCollideable(ICollideable coll)
        {
            for (int i = 0; i < coll.CollisionGeometry.Count; i++)
            {
                var geo = coll.CollisionGeometry[i];

                geo.OnCollision -= onCollision;
                geo.Dispose();
            }
            /*
            foreach (var geo in coll.CollisionGeometry)
            {
                geo.OnCollision -= onCollision;
                continue;
            }
            */
        }

        public void Initialize()
        {
            OnCollidedWith.DoAutoRegister();
        }
    }
}
