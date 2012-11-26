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
using System.IO;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.States
{
    public class LevelManager
    {
        public LevelManager(CollisionManager collisionsManager, PhysicsManager physicsManager, RenderManager renderManager)
        {
            CollisionManager = collisionsManager;
            PhysicsManager = physicsManager;
            RenderManager = renderManager;

            DeserializeLevel();
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
            using (var xmlWriter = XmlWriter.Create("test.txt"))
            {
                var serializer = new XmlSerializer(typeof(Level));
                serializer.Serialize(xmlWriter, Level);

                xmlWriter.Flush();
            }
        }

        public void DeserializeLevel()
        {
            if (File.Exists("test.txt"))
            {
                using (var xmlReader = XmlReader.Create("test.txt"))
                {
                    var deserializer = new XmlSerializer(typeof(Level));
                    Level = deserializer.Deserialize(xmlReader) as Level;

                    RegisterLevelComponents();
                }
            }
            else
            {
                Level = new Level();

                RegisterTiles(new List<Tile>()
                {
                    new Tile("Textures/Ground_Tile1", new Vector2(187, 361.5f), 0, Vector2.One),
                    new Tile("Textures/Ground_Tile1", new Vector2(461, 361.5f), 0, Vector2.One),
                    new Tile("Textures/Ground_Tile1", new Vector2(735, 361.5f), 0, Vector2.One),
                    new Tile("Textures/Side_Tile01", new Vector2(1009, 361.5f), 0, Vector2.One),
                    new Tile("Textures/Top_Tile01", new Vector2(187, 293.5f), 0, Vector2.One),
                    new Tile("Textures/Top_Tile01", new Vector2(461, 293.5f), 0, Vector2.One),
                    new Tile("Textures/Top_Tile01", new Vector2(735, 293.5f), 0, Vector2.One),
                });
            }
        }

        private void RegisterLevelComponents()
        {
            Level.Tiles.ForEach(x => RenderManager.RegisterRenderable(x));
            Level.Entities.ForEach(
                x =>
                {
                    PhysicsManager.RegisterPhysicsBody(x);
                    CollisionManager.RegisterCollideable(x);
                    RenderManager.RegisterRenderable(x);
                });
        }
    }
}
