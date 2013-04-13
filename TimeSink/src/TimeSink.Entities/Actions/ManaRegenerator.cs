using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.StateManagement;

namespace TimeSink.Entities.Actions
{
    [EditorEnabled]
    [SerializableEntity("e488c785-4483-43ae-95b5-d839c6d2089f")]
    public class ManaRegenerator : InteractableItem
    {
        const string EDITOR_NAME = "Mana Regenerator";
        const float REGEN_RATE = .015f;

        private static readonly Guid guid = new Guid("e488c785-4483-43ae-95b5-d839c6d2089f");
        private float mana = 100;

        public ManaRegenerator()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public ManaRegenerator(Vector2 position, int width, int height)
            : base(position, width, height)
        {
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

        protected override void ExecuteHeldAction(GameTime gameTime)
        {
            if (Character.Mana < UserControlledCharacter.MAX_MANA)
            {
                var manaUsage = Math.Min(mana, REGEN_RATE * gameTime.ElapsedGameTime.Milliseconds);
                Character.Mana = Math.Min(UserControlledCharacter.MAX_MANA, Character.Mana + manaUsage);
                mana -= manaUsage;
                Engine.UpdateHealth();
            }

            if (mana == 0)
            {
                used = true;
            }
        }
    }
}
