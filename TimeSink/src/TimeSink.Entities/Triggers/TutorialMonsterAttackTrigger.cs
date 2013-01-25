using Engine.Defaults;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Entities.Enemies;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Triggers
{
    [EditorEnabled]
    [SerializableEntity("7e3fb39c-dd88-4a22-a4bd-b2c77f6f930f")]
    public class TutorialMonsterAttackTrigger : Trigger
    {
        const string EDITOR_NAME = "Tutorial Monster Attack Trigger";

        private static readonly Guid GUID = new Guid("7e3fb39c-dd88-4a22-a4bd-b2c77f6f930f");

        public TutorialMonsterAttackTrigger()
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
            Physics.RegisterOnCollidedListener<TutorialMonster>(OnCollidedWith);
        }

        private bool used;
        public virtual bool OnCollidedWith(Fixture f, TutorialMonster monster, Fixture f2, Contact info)
        {
            if (!used)
            {
                monster.RevJoint.MotorSpeed = 0;
                monster.RevJoint.MotorTorque = 0;

                used = true;
            }

            return true;
        }
    }
}
