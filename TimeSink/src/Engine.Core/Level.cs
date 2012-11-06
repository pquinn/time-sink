using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core
{
    public class Level
    {
        public Level()
        {
        }

        public List<StaticMesh> StaticMeshes { get; set; }

        public List<Entity> Entities { get; set; }
    }
}
