using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;

namespace TimeSink.Engine.Core.Collisions
{
    public static class CollisionUtils
    {
        public delegate bool CollisionDelegate<T>(Fixture f1, T collidedWith, Fixture f2, Contact contact) where T : Entity;
        public delegate void SeparationDelegate<T>(Fixture f1, T e2, Fixture f2) where T : Entity;

        public static void RegisterOnCollidedListener<T>(this Body physBody, CollisionDelegate<T> callback) where T : Entity
        {
            physBody.OnCollision += delegate(Fixture f1, Fixture f2, Contact c)
            {
                if (f1.Body == physBody && f2.Body.UserData is T)
                    c.Enabled = callback(f1, f2.Body.UserData as T, f2, c);
                else if (f1.Body.UserData is T)
                    c.Enabled = callback(f2, f1.Body.UserData as T, f1, c);

                return c.Enabled;
            };
        }

        public static void RegisterOnCollidedListener<T>(this Fixture f, CollisionDelegate<T> callback) where T : Entity
        {
            f.OnCollision += delegate(Fixture f1, Fixture f2, Contact c)
            {
                if (f1 == f && f2.Body.UserData is T)
                    return callback(f1, f2.Body.UserData as T, f2, c);
                else if (f1.Body.UserData is T)
                    return callback(f2, f1.Body.UserData as T, f1, c);

                return true;
            };
        }

        public static void RegisterOnSeparatedListener<T>(this Body physBody, SeparationDelegate<T> callback) where T : Entity
        {
            physBody.OnSeparation += delegate(Fixture f1, Fixture f2)
            {
                if (f1.Body == physBody && f2.Body.UserData is T)
                    callback(f1, f2.Body.UserData as T, f2);
                else if (f1.Body.UserData is T)
                    callback(f2, f1.Body.UserData as T, f1);
            };
        }

        public static void RegisterOnSeparatedListener<T>(this Fixture f, SeparationDelegate<T> callback) where T : Entity
        {
            f.OnSeparation += delegate(Fixture f1, Fixture f2)
            {
                if (f1 == f && f2.Body.UserData is T)
                    callback(f1, f2.Body.UserData as T, f2);
                else if (f1.Body.UserData is T)
                    callback(f2, f1.Body.UserData as T, f1);
            };
        }
    }
}
