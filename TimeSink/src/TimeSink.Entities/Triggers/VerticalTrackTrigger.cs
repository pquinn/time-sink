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
        private static readonly Guid guid = new Guid("c4c2f0bb-91ed-41ac-a905-b126068b9c31");
        private const string EDITOR_NAME = "Vertical Track Trigger";

        [EditableField("Enemy")]
        [SerializableField]
        public string EnemyString { get; set; }

        private VerticalTracker Enemy { get; set; }


        public VerticalTrackTrigger()
            : this("")
        { }

        public VerticalTrackTrigger(string enemy)
            : base()
        {
            EnemyString = enemy;
            //InitializeEnemy();

        }

        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
 	        base.InitializePhysics(force, engineRegistrations);

            var target = 
                Engine == null ? null : Engine.LevelManager.Level.Entities.First(x => x.InstanceId.Equals(EnemyString)) as VerticalTracker;

            if (target != null)
            {
                Enemy = target;
            }
        }

        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
            Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public virtual bool OnCollidedWith(Fixture f, UserControlledCharacter monster, Fixture f2, Contact info)
        {
            Enemy.Descend();
            return true;
        }

        public virtual void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            if (c.Position.Y < Position.Y)
            {
                Enemy.Jump();
            }
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
