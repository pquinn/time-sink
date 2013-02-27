using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Entities.Actions;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Collisions;
using Engine.Defaults;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Triggers

{
    [EditorEnabled]
    [SerializableEntity("4397882b-d86d-4c6a-89ac-09ed14babdcf")]
    public class QuestCompletionTrigger : Trigger
    {
        const string EDITOR_NAME = "Quest Completion Trigger";
        private static readonly Guid guid = new Guid("4397882b-d86d-4c6a-89ac-09ed14babdcf");
        private static readonly Vector2 SrcRectSize = new Vector2(200f, 304f);

        public QuestCompletionTrigger()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public QuestCompletionTrigger(Vector2 position, int width, int height)
            : base(position, width, height)
        {
        }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }


        public override IRendering Preview
        {
            get
            {
                return new NullRendering();
            }
        }
        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() { new NullRendering() };
            }
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        [EditableField("Quest Key")]
        public string QuestKey { get; set; }

        private bool triggered;

        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            if (!triggered)
            {
                levelManager.LevelCache.Add(QuestKey, true);
                triggered = true;
            }

            return true;
        }
    }
}
