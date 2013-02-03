using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("cc365414-a0c7-42f1-b9c1-c716f3f5239a")]
    public class CentipedeKiller : EnemyKiller<NormalCentipede>
    {
        const float DEPTH = -75f;
        private static readonly Guid guid = new Guid("cc365414-a0c7-42f1-b9c1-c716f3f5239a");
        private static readonly string editorName = "Centipede Killer";

        public override string EditorName
        {
            get { return editorName; }
        }

        public override System.Guid Id
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
