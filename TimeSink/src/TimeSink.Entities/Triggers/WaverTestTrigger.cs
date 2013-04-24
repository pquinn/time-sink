using Autofac;
using Engine.Defaults;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Entities.Enemies;

namespace TimeSink.Entities.Triggers
{
    [EditorEnabled]
    [SerializableEntity("2605fa3e-389a-4ee3-a0d0-d576ab189404")]
    public class WaverTestTrigger : Trigger
    {
        const string EDITOR_NAME = "Wave Test Trigger";
        private static readonly Guid GUID = new Guid("2605fa3e-389a-4ee3-a0d0-d576ab189404");

        public WaverTestTrigger()
            : base()
        {
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }
       
        protected override void RegisterCollisions()
        {            
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
        }

        private bool used;
        public virtual bool OnCollidedWith(Fixture f, UserControlledCharacter obj, Fixture f2, Contact info)
        {
            if (!used)
            {
                var waver = new Waver(Position - new Vector2(0, PhysicsConstants.PixelsToMeters(800)));
                levelManager.RegisterEntity(waver);

                used = true;
            }

            return true;
        }
    }
}
