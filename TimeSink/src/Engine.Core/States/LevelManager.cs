using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Collision.Shapes;
using System.Xml.Serialization;
using System.Xml;

namespace TimeSink.Engine.Core.States
{
    public class LevelManager
    {
        public LevelManager(CollisionManager collisionsManager, PhysicsManager physicsManager, RenderManager renderManager, Level level)
        {
            CollisionManager = collisionsManager;
            PhysicsManager = physicsManager;
            RenderManager = renderManager;
            Level = level;

            RegisterLevelComponents();
        }

        private void RegisterLevelComponents()
        {
            RegisterTiles(Level.Tiles);
            RegisterEntities(Level.Entities);
        }

        public CollisionManager CollisionManager { get; private set; }

        public PhysicsManager PhysicsManager { get; private set; }

        public RenderManager RenderManager { get; private set; }

        public Level Level { get; set; }

        public void RegisterTile(Tile mesh)
        {
            Level.Tiles.Add(mesh);
            RenderManager.RegisterRenderable(mesh);
        }

        public void RegisterTiles(IEnumerable<Tile> meshes)
        {
            meshes.ForEach(RegisterTile);
        }

        public void RegisterEntity(Entity entity)
        {
            Level.Entities.Add(entity);
            PhysicsManager.RegisterPhysicsBody(entity);
            CollisionManager.RegisterCollideable(entity);
            RenderManager.RegisterRenderable(entity);
        }

        public void RegisterEntities(IEnumerable<Entity> entities)
        {
            entities.ForEach(RegisterEntity);
        }

        public void SerializeLevel()
        {
            using (var xmlWriter = XmlWriter.Create("test"))
            {
                var serializer = new XmlSerializer(typeof(Level));
                serializer.Serialize(xmlWriter, Level);

                xmlWriter.Flush();
            }
        }
    }
}
