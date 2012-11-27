using System.Collections.Generic;
using System;

namespace TimeSink.Engine.Core.States
{
    public class EntitySerialization
    {
        public EntitySerialization()
            : this(new Guid(), new List<Pair<string,object>>())
        {
        }

        public EntitySerialization(Guid entityId)
            : this(entityId, new List<Pair<string,object>>())
        {
        }

        public EntitySerialization(Guid entityId, List<Pair<string, object>> propertiesMap)
        {
            EntityId = entityId;
            PropertiesMap = propertiesMap;
        }

        public Guid EntityId { get; set; }

        public List<Pair<string, object>> PropertiesMap { get; private set; }
    }
}
