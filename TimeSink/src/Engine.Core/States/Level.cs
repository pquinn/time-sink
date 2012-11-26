using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using System.Collections.Generic;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using System.Xml.Serialization;

namespace TimeSink.Engine.Core
{
    public class Level
    { 
        public Level()
        {
            Tiles = new List<Tile>();
            Entities = new List<Entity>();
            CollisionGeometry = new List<LoopShape>();
        }

        public List<Tile> Tiles { get; set; }

        [XmlIgnore]
        public List<Entity> Entities { get; set; }

        [XmlIgnore]
        public List<LoopShape> CollisionGeometry { get; private set; }
    }
}
