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
using TimeSink.Engine.Core.Editor;
using Autofac;

namespace TimeSink.Engine.Core.States
{
    public class LevelManager
    {
        public LevelManager(CollisionManager collisionsManager, PhysicsManager physicsManager, 
            RenderManager renderManager, EditorRenderManager editorRenderManager, IComponentContext container)
        {
            CollisionManager = collisionsManager;
            PhysicsManager = physicsManager;
            RenderManager = renderManager;
            EditorRenderManager = editorRenderManager;
            Container = container;
            Level = new Level();
        }

        public CollisionManager CollisionManager { get; private set; }

        public PhysicsManager PhysicsManager { get; private set; }

        public RenderManager RenderManager { get; private set; }

        public EditorRenderManager EditorRenderManager { get; private set; }

        public IComponentContext Container { get; private set; }

        public Level Level { get; set; }

        public void RegisterMidground(Tile tile)
        {
            Level.Midground.Add(tile);
            RenderManager.RegisterRenderable(tile);
            EditorRenderManager.RegisterPreviewable(tile);
        }

        public void RegisterMidground(IEnumerable<Tile> tiles)
        {
            tiles.ForEach(RegisterMidground);
        }

        public void RegisterTile(Tile tile)
        {
            Level.Tiles.Add(tile);
            RenderManager.RegisterRenderable(tile);
            EditorRenderManager.RegisterPreviewable(tile);
        }

        public void RegisterTiles(IEnumerable<Tile> tiles)
        {
            tiles.ForEach(RegisterTile);
        }

        public void RegisterEntity(Entity entity)
        {
            Level.Entities.Add(entity);
            PhysicsManager.RegisterPhysicsBody(entity);
            CollisionManager.RegisterCollideable(entity);
            RenderManager.RegisterRenderable(entity);
            EditorRenderManager.RegisterPreviewable(entity);
        }

        public void RegisterEntities(IEnumerable<Entity> entities)
        {
            entities.ForEach(RegisterEntity);
        }

        public void SerializeLevel(string fileName)
        {
            using (var xmlWriter = XmlWriter.Create(fileName))
            {
                Level.FlushEntities();

                var serializer = new XmlSerializer(typeof(Level));
                serializer.Serialize(xmlWriter, Level);

                xmlWriter.Flush();
            }
        }

        public void DeserializeLevel(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (var xmlReader = XmlReader.Create(fileName))
                {
                    var deserializer = new XmlSerializer(typeof(Level));
                    Level = deserializer.Deserialize(xmlReader) as Level;

                    Level.ExtractEntities(Container);

                    RegisterLevelComponents();
                }
            }
        }

        public void Clear()
        {
            RenderManager.Clear();
            EditorRenderManager.Clear();
        }

        private void RegisterLevelComponents()
        {
            Level.Midground.ForEach(
                x =>
                {
                    RenderManager.RegisterRenderable(x);
                    EditorRenderManager.RegisterPreviewable(x);
                });

            Level.Tiles.ForEach(
                x =>
                {
                    RenderManager.RegisterRenderable(x);
                    EditorRenderManager.RegisterPreviewable(x);
                });

            Level.Entities.ForEach(
                x =>
                {
                    PhysicsManager.RegisterPhysicsBody(x);
                    CollisionManager.RegisterCollideable(x);
                    RenderManager.RegisterRenderable(x);
                    EditorRenderManager.RegisterPreviewable(x);
                });
        }
    }
}
