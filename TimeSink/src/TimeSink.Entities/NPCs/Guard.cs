using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.StateManagement;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.NPCs
{
    [EditorEnabled]
    [SerializableEntity("7fc342e9-653e-44b6-9762-5527d7bb4eba")]
    public class Guard : NonPlayerCharacter
    {
        const string EDITOR_NAME = "Guard Wall";
        private static readonly Guid GUID = new Guid("7fc342e9-653e-44b6-9762-5527d7bb4eba");

        public Guard()
            : base()
        {
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [EditableField("Passable?")]
        public bool Passable { get; set; }

        [SerializableField]
        [EditableField("Passable Texture Name")]
        public string PassableTextureName { get; set; }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            base.InitializePhysics(false, engineRegistrations);
            if (force || !initialized)
            {
                if (!Passable)
                {
                    var fix = Physics.FixtureList[0];
                    fix.CollidesWith = Category.All;
                }
                initialized = true;
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            if (collided && DialogueRootsList.Count > 0)
            {
                if (InputManager.Instance.IsNewKey(Keys.X) && !game.ScreenManager.IsInDialogueState())
                {
                    game.ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid(DialogueRootsList[DialogueState])), null);
                    Passable = true;
                }
            }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (Passable)
            {
                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat1;

                if (PassableTextureName != String.Empty)
                    TextureName = PassableTextureName;
            }
            base.OnUpdate(time, world);
        }
    }
}
