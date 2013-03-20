using Engine.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Entities.Enemies;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;

namespace TimeSink.Entities.Triggers
{
    [SerializableEntity("c4c2f0bb-91ed-41ac-a905-b126068b9c31")]
    [EditorEnabled]
    public class VerticalTrackTrigger : Trigger
    {

        const string EDITOR_NAME = "Vertical Track Trigger";

        private static readonly Guid guid = new Guid("c4c2f0bb-91ed-41ac-a905-b126068b9c31");

        [EditableField("Enemy")]
        [SerializableField]
        public string Enemy { get; set; }


        public VerticalTrackTrigger()
            : this("")
        { }

        public VerticalTrackTrigger(string enemy)
            : base()
        {
            Enemy = enemy;
        }

        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public virtual bool OnCollidedWith(Fixture f, UserControlledCharacter monster, Fixture f2, Contact info)
        {

            return true;
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
