using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.StateManagement;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Objects
{
    [EditorEnabled]
    [SerializableEntity("e498c785-4483-43ae-95b5-d839c6d2089f")]
    public class Chest : InteractableItem
    {
        const string EDITOR_NAME = "Chest";

        private static readonly Guid guid = new Guid("e498c785-4483-43ae-95b5-d839c6d2089f");

        public Chest()
            : this(Vector2.Zero, 50, 50, String.Empty)
        {
        }

        public Chest(Vector2 position, int width, int height, string prompt)
            : base(position, width, height)
        {
            Prompt = prompt;
        }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        [EditableField("Prompt")]
        public string Prompt { get; set; }

        protected override void ExecuteAction()
        {
            base.ExecuteAction();

            var guard = (NonPlayerCharacter)engine.LevelManager.Level.Entities.First(x => x.InstanceId.Equals("town_guard"));
            guard.DialogueState++;

            if (!engine.ScreenManager.IsInDialogueState())
                engine.ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid(Prompt)), null);
        }
    }
}
