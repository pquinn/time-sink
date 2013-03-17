using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core;

namespace TimeSink.Entities.Objects
{
    public abstract class Wall : Entity
    {
        public bool BulletPassable { get; set; }

        private void OnCollidedWith(Fixture f1, Projectile proj, Fixture f2, Contact contact)
        {

        }
    }
}
