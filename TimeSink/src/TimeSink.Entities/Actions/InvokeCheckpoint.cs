using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Entities.Actons;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;

namespace TimeSink.Entities.Actions
{
    [EditorEnabled]
    [SerializableEntity("5487de5f-acba-42c9-9404-b05ddea64b02")]
    public class InvokeCheckpoint : InteractableItem
    {
        const string EDITOR_NAME = "Checkpoint";
        private static readonly Guid guid = new Guid("5487de5f-acba-42c9-9404-b05ddea64b02");

        public InvokeCheckpoint()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public InvokeCheckpoint(Vector2 position, int width, int height)
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

        [SerializableField]
        [EditableField("Spawn Point")]
        public int SpawnPoint { get; set; }

        protected override void ExecuteAction()
        {
            engine.LevelManager.LevelCache.ReplaceOrAdd(
                "Save",
                new Save(Engine.LevelManager.LevelPath, SpawnPoint, Character.Health, Character.Mana, Character.Inventory));
        }
    }
}
