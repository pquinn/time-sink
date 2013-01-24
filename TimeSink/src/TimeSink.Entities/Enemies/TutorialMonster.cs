using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("fc8cddb4-e1ef-4b83-bc22-5b6460103524")]
    public class TutorialMonster : Enemy
    {
        const float MASS = 100f;
        const string EDITOR_NAME = "Tutorial Monster";

        private static readonly Guid GUID = new Guid("fc8cddb4-e1ef-4b83-bc22-5b6460103524");

        public TutorialMonster()
            : this(Vector2.Zero, Vector2.Zero)
        {
        }

        public TutorialMonster(Vector2 position, Vector2 direction)
            : base(position)
        {
            DirectionFacing = direction;
        }

        [EditableField("Direction Facing")]
        [SerializableField]
        public Vector2 DirectionFacing { get; set; }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }
    }
}
