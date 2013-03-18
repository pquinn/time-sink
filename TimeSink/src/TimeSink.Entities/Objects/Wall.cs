using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Entities.Inventory;

namespace TimeSink.Entities.Objects
{
    public abstract class Wall : Entity
    {
        [SerializableField]
        [EditableField("Bullet Passable?")]
        public bool BulletPassable { get; set; }

        public virtual bool OnCollidedWith(Fixture f1, Arrow proj, Fixture f2, Contact contact)
        {
            if (BulletPassable)
            {
                return false;
            }
            else
            {
                // do stuff
                return true;
            }
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            Physics.RegisterOnCollidedListener<Arrow>(OnCollidedWith);
            base.InitializePhysics(force, engineRegistrations);
        }
    }
}
