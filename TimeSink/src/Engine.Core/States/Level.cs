using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using System.Collections.Generic;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Linq;
using FarseerPhysics.Common;

namespace TimeSink.Engine.Core
{
    public class Level
    {
        private bool isGeoDirty;
        private List<LoopShape> geoCache;

        public Level()
        {
            Tiles = new List<Tile>();
            Entities = new List<Entity>();
        }

        public List<Tile> Tiles { get; set; }

        public List<List<Vector2>> GeoChains { get; set; }

        [XmlIgnore]
        public List<Entity> Entities { get; set; }

        [XmlIgnore]
        public List<LoopShape> CollisionGeometry
        {
            get
            {
                if (!isGeoDirty)
                {
                    return geoCache;
                }
                else
                {
                    return geoCache =
                        GeoChains.Select(x => new LoopShape(new Vertices(x))).ToList();
                }
            }
        }
    }
}
