using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Actions
{
    [EditorEnabled]
    [SerializableEntity("2b4b670a-71fe-46bd-b4b9-d413fe830713")]
    public class ConditionalDoor : UseDoor
    {
        public string EDITOR_NAME = "Conditional Door";

        private static readonly Guid guid = new Guid("2b4b670a-71fe-46bd-b4b9-d413fe830713");

        public ConditionalDoor()
            : this(Vector2.Zero, 50, 50, DoorType.Up, string.Empty, 0)
        {
        }

        public ConditionalDoor(Vector2 position, int width, int height, DoorType doorType, string levelPath, int spawnPoint)
            : base(position, width, height, doorType, levelPath, spawnPoint)
        {
            enabled = false;
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        [SerializableField]
        [EditableField("Condition Key")]
        public string ConditionKey { get; set; }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (!enabled) 
            {
                object result;
                world.LevelManager.LevelCache.TryGetValue(ConditionKey, out result);
                if (result != null) enabled = (bool)result;
            }

            base.OnUpdate(time, world);
        }
    }
}
