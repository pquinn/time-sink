using Engine.Defaults;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using Autofac;
using FarseerPhysics.Factories;

namespace TimeSink.Entities.Triggers
{
    [SerializableEntity("ce305111-8ab2-4e54-b737-b932a1d5d127")]
    [EditorEnabled]
    class CameraUnlockTrigger : Trigger
    {
        const String EDITOR_NAME = "Camera_Unlock_Trigger";
        private static readonly Guid guid = new Guid("ce305111-8ab2-4e54-b737-b932a1d5d127");
        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public override Guid Id
        {
            get
            {
                return guid;
            }
            set{}
        }

        public virtual bool OnCollidedWith(Fixture f, UserControlledCharacter obj, Fixture f2, Contact info)
        {
            if (Engine.CameraLock)
            {
                Engine.CameraLock = false;
            }
            return false;
        }

    }
}
