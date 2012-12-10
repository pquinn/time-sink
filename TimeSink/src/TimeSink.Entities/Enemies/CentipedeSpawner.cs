using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("349aaec2-aa55-4c37-aa71-42d0c1616885")]
    public class CentipedeSpawner : EnemySpawner<NormalCentipede>
    {
        public CentipedeSpawner(float interval, int max) : base(interval, max) { }

        protected override NormalCentipede SpawnEnemy(GameTime time, EngineGame world)
        {
            return new NormalCentipede(Position, Vector2.UnitX);
        }
    }
}
