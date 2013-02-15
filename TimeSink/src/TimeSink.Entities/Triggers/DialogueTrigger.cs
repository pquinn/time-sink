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
using TimeSink.Engine.Core.StateManagement;

namespace TimeSink.Entities.Triggers
{
    [EditorEnabled]
    [SerializableEntity("729f6f88-9ce0-4c66-ab70-b8643f982ae2")]
    class DialogueTrigger : Trigger
    {
        const string EDITOR_NAME = "Dialogue Trigger";
        private static readonly Guid GUID = new Guid("729f6f88-9ce0-4c66-ab70-b8643f982ae2");

        public DialogueTrigger()
            : base()
        {
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        [EditableField("Dialogue Start")]
        public string DialogueId { get; set; }

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
                if (!engineGame.ScreenManager.IsInDialogueState() && DialogueId != null)
                {
                    engineGame.ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid(DialogueId)), null);
                    var tutorialTrigger = engineGame.LevelManager.Level.Entities.First(x => x.InstanceId.Equals("movement info")) as TutorialTrigger;
                    tutorialTrigger.Active = true;
                    tutorialTrigger.RecheckCollision();
                    used = true;
                }
            }

            return true;
        }
    }
}
