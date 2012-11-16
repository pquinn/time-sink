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
                geo.OnCollision += onCollision;
        }

        private static bool onCollision(Fixture f1, Fixture f2, Contact contact)
        {
            OnCollidedWith.Invoke(f1.Body.UserData as ICollideable, f2.Body.UserData as ICollideable, contact);
            return true;
        }

        public void UnregisterCollideable(ICollideable coll)
        {
            foreach (var geo in coll.CollisionGeometry)
                geo.OnCollision -= onCollision;
        }

        public void Initialize()
        {
            //Collided.DoAutoRegister();
            OnCollidedWith.DoAutoRegister();
        }


        //public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        //{
        //    spriteBatch.Begin();

        //    foreach (var collideable in collideables)
        //    {
        //        collideable.CollisionGeometry.Draw(spriteBatch, cache, globalTransform);
        //    }

        //    spriteBatch.End();
        //}
    }
}
