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
using TimeSink.Engine.Core.States;
using Autofac;
using System;

namespace TimeSink.Engine.Core
{
    public class Level
    {
        private bool isGeoDirty = true;

        public Level()
        {
            Tiles = new List<Tile>();
            Entities = new List<Entity>();
            Midground = new List<Tile>();
            GeoSegments = new List<List<WorldCollisionGeometrySegment>>() { new List<WorldCollisionGeometrySegment>() };
        }

        public Vector2 PlayerStart { get; set; }

        public List<Tile> Midground { get; set; }

        public List<Tile> Tiles { get; set; }

        public List<List<WorldCollisionGeometrySegment>> GeoSegments { get; set; }

        public List<EntitySerialization> EntitySerializations { get; set; }

        [XmlIgnore]
        public List<Entity> Entities { get; set; }

        public void FlushEntities()
        {
            EntitySerializations = Entities.Select(
                entity =>
                {
                    var type = entity.GetType();
                    var properties = type.GetProperties();
                    var dictionary = new Dictionary<string, object>();
                    properties.ForEach(
                        prop =>
                        {
                            var attributes = prop.GetCustomAttributes(typeof(SerializableFieldAttribute), false).ToList();
                            if (attributes.Any())
                            {
                                dictionary.Add(prop.Name, prop.GetValue(entity, null));
                            }
                        });

                    var id = type.GetCustomAttributes(typeof(SerializableEntityAttribute), false).First() as SerializableEntityAttribute;

                    return new EntitySerialization(
                        id.Id, 
                        dictionary.Select(i => new Pair<string, object>(){ Key = i.Key, Value = i.Value }).ToList());
                }).ToList();
        }

        public void ExtractEntities(IComponentContext container)
        {
            Entities.Clear();
            var entities = container.Resolve<IEnumerable<Entity>>().ToList();
            EntitySerializations.ForEach(
                x =>
                {
                    var entity = entities.First(e => e.Id == x.EntityId);
                    var type = entity.GetType();
                    var clone = Activator.CreateInstance(type) as Entity;
                    x.PropertiesMap.ForEach(
                        p =>
                        {
                            type.GetProperty(p.Key).SetValue(clone, p.Value, null);
                        });

                    Entities.Add(clone);
                });
        }
    }
}
