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
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;

namespace TimeSink.Engine.Core.States
{
    public delegate void LevelLoadedEventHandler();
    public delegate void GeometryResetEventHandler();

    public class LevelManager
    {
        public LevelManager(PhysicsManager physicsManager, RenderManager renderManager, IComponentContext container)
        {
            PhysicsManager = physicsManager;
            RenderManager = renderManager;
            Container = container;
            Level = new Level();
            LevelCache = new Dictionary<string, object>();

            Physics = BodyFactory.CreateBody(PhysicsManager.World, Vector2.Zero);

            PhysicsManager.TimeScaleLookup = PhysicsManager.TimeScaleLookup =
                position =>
                    Level.TimeScaleCircles.Aggregate(
                        1f,
                        (accumulator, circle) =>
                            accumulator * Math.Max(
                                1f, 
                                Vector2.DistanceSquared(circle.Center, position) / (circle.Radius * circle.Radius)));
        }

        public PhysicsManager PhysicsManager { get; private set; }

        public RenderManager RenderManager { get; private set; }

        public IComponentContext Container { get; private set; }

        public Level Level { get; set; }

        public string LevelPath { get; set; }

        public LevelLoadedEventHandler LevelLoaded { get; set; }

        public GeometryResetEventHandler GeometryReset { get; set; }

        public Dictionary<string, object> LevelCache { get; set; }

        public void RegisterMidground(Tile tile)
        {
            Level.Midground.Add(tile);
            RenderManager.RegisterRenderable(tile);
        }

        public void RegisterMidground(IEnumerable<Tile> tiles)
        {
            tiles.ForEach(RegisterMidground);
        }

        public void RegisterBackground(Tile tile)
        {
            Level.Background.Add(tile);
            RenderManager.RegisterRenderable(tile);
        }

        public void RegisterBackground(IEnumerable<Tile> tiles)
        {
            tiles.ForEach(RegisterBackground);
        }

        public void RegisterTile(Tile tile)
        {
            Level.Tiles.Add(tile);
            RenderManager.RegisterRenderable(tile);
        }

        public void RegisterTiles(IEnumerable<Tile> tiles)
        {
            tiles.ForEach(RegisterTile);
        }

        public void UnregisterTile(Tile tile)
        {
            RenderManager.UnregisterRenderable(tile);
            Level.Tiles.Remove(tile);
        }

        public void UnregisterTiles(IEnumerable<Tile> tiles)
        {
            tiles.ForEach(x => UnregisterTile(x));
        }

        public void RegisterEntity(Entity entity)
        {
            Level.Entities.Add(entity);
            PhysicsManager.RegisterPhysicsBody(entity);
            RenderManager.RegisterRenderable(entity);
        }

        public void RegisterEntities(IEnumerable<Entity> entities)
        {
            entities.ForEach(RegisterEntity);
        }

        public void UnregisterEntity(Entity entity)
        {
            Level.Entities.Remove(entity);
            PhysicsManager.UnregisterPhysicsBody(entity);
            RenderManager.UnregisterRenderable(entity);
        }

        public void UnregisterEntities(IEnumerable<Entity> entities)
        {
            entities.ForEach(UnregisterEntity);
        }

        public void SerializeLevel(string fileName)
        {
            using (var xmlWriter = XmlWriter.Create(fileName))
            {
                var worldGeo = Level.Entities.FindAll(x => x is WorldGeometry2);
                UnregisterEntities(worldGeo);

                Level.FlushEntities();

                var serializer = new XmlSerializer(typeof(Level));
                serializer.Serialize(xmlWriter, Level);

                xmlWriter.Flush();

                ResetGeometry();
            }
        }

        public void DeserializeLevel(string fileName)
        {
            if (File.Exists(fileName))
            {
                var levelWithExt = fileName.Split(new string[]{ "Levels\\" }, StringSplitOptions.None)[1];
                LevelPath = levelWithExt.Substring(0, levelWithExt.IndexOf('.'));
                using (var xmlReader = XmlReader.Create(fileName))
                {
                    var deserializer = new XmlSerializer(typeof(Level));
                    Level = deserializer.Deserialize(xmlReader) as Level;

                    Level.ExtractEntities(Container);

                    RegisterLevelComponents();
                }

                if (LevelLoaded != null)
                    LevelLoaded();
            }
            else
                throw new FileNotFoundException(
                    string.Format("Could not deserialize level '{0}'", fileName));
        }

        public void Clear()
        {
            Level = new Level();
            PhysicsManager.Clear();
            RenderManager.Clear();
            ResetGeometry();
        }

        private void RegisterLevelComponents()
        {   
            Level.Midground.ForEach(
                x =>
                {
                    RenderManager.RegisterRenderable(x);
                });

            Level.Background.ForEach(
                x =>
                {
                    RenderManager.RegisterRenderable(x);
                });

            Level.Tiles.ForEach(
                x =>
                {
                    RenderManager.RegisterRenderable(x);
                });

            Level.Entities.ForEach(
                x =>
                {
                    PhysicsManager.RegisterPhysicsBody(x);
                    RenderManager.RegisterRenderable(x);
                });

            RegisterEntity(new WorldGeometry2() { GeoChains = Level.GeoSegments });
        }

        public Body Physics;
        private List<Fixture> fixtures = new List<Fixture>();

        public void ResetGeometry()
        {
            fixtures.ForEach(x => x.Dispose());
            fixtures.Clear();

            var worldGeo = Level.Entities.FindAll(x => x is WorldGeometry2);
            UnregisterEntities(worldGeo);

            RegisterEntity(new WorldGeometry2() { GeoChains = Level.GeoSegments });

            if (GeometryReset != null)
            {
                GeometryReset();
            }
        }
    }
}
