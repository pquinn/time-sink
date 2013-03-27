using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Defaults;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Entities.Objects;

namespace TimeSink.Entities.Triggers
{
    [SerializableEntity("60f7d4d9-287c-4ade-98c0-1e6e9023a812")]
    [EditorEnabled]
    public class WiredVineTrigger : Trigger
    {
        const String EDITOR_NAME = "Wired Vine Trigger";
        private static readonly Guid guid = new Guid("60f7d4d9-287c-4ade-98c0-1e6e9023a812");
        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
        }

        public virtual bool OnCollidedWith(Fixture f, UserControlledCharacter obj, Fixture f2, Contact info)
        {
            var bridge = (WireBridge) engineGame.LevelManager.Level.Entities.First(x => x.InstanceId == WireBridgeId);
            bridge.ElectrifyWire();

            return true;
        }

        [SerializableField]
        [EditableField("Wire Bridge Id")]
        public string WireBridgeId { get; set; }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override Guid Id
        {
            get
            {
                return guid;
            }
            set
            {
            }
        }
    }
}
