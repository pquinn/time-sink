using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Enemies;

namespace TimeSink.Entities.Actions
{

    [EditorEnabled]
    [SerializableEntity("94fe6bbb-46ea-4b52-b061-f0776e11b42e")]
    class BurnNest : InteractableItem
    {
        const string EDITOR_NAME = "Burn Nest";

        private static readonly Guid guid = new Guid("94fe6bbb-46ea-4b52-b061-f0776e11b42e");

        public BurnNest()
            : this(Vector2.Zero, 50, 50, String.Empty)
        {
        }

        public BurnNest(Vector2 position, int width, int height, string Nest)
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
        [EditableField("Nest")]
        public string Nest { get; set; }

        protected override void ExecuteAction()
        {
            base.ExecuteAction();

            var nest = (CentipedeSpawner)engine.LevelManager.Level.Entities.First(x => x.InstanceId.Equals(Nest));
            if (Character.HoldingTorch != null)
            {
                nest.Dead = true;
                used = true;
            }
        }
    }
}
