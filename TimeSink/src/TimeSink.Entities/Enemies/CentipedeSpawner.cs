using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("349aaec2-aa55-4c37-aa71-42d0c1616885")]
    public class CentipedeSpawner : EnemySpawner<NormalCentipede>
    {
        const string TEXTURE = "Textures/Enemies/Nest";
        const string EDITOR_NAME = "Centipede Spawner";
        const float DEPTH = -75f;

        public CentipedeSpawner() : base() { }
        public CentipedeSpawner(float interval, float offset, int max, int width, int height, int dir) : base(interval, offset, max, width, height)
        {
            SpawnDirection = dir;
        }

        [EditableField("Direction")]
        [SerializableField]
        public int SpawnDirection { get; set; }

        protected override NormalCentipede SpawnEnemy(GameTime time, EngineGame world)
        {
            return new NormalCentipede(Position, Vector2.UnitX * (SpawnDirection > 0 ? 1 : -1));
        }

        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Size = new Vector2(Width, Height),
                    DepthWithinLayer = DEPTH
                };
            }
        }
        public override IRendering Preview
        {
            get
            {
                return Rendering;
            }
        }
        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }
    }
}
