using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using System.Collections.Generic;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core
{
    public class Level
    {
        private List<Tile> staticMeshes;
        private List<Entity> entities;

        public Level(CollisionManager collisionsManager, PhysicsManager physicsManager, RenderManager renderManager)
        {
            staticMeshes = new List<Tile>();
            entities = new List<Entity>();
            CollisionManager = collisionsManager;
            PhysicsManager = physicsManager;
            RenderManager = renderManager;
            CollisionGeometry = new List<LoopShape>();
        }

        public CollisionManager CollisionManager { get; private set; }

        public PhysicsManager PhysicsManager { get; private set; }

        public RenderManager RenderManager { get; private set; }

        public void RegisterStaticMesh(Tile mesh)
        {
            staticMeshes.Add(mesh);
            CollisionManager.RegisterCollideable(mesh);
            PhysicsManager.RegisterPhysicsBody(mesh);
            RenderManager.RegisterRenderable(mesh);
        }

        public void RegisterStaticMeshes(IEnumerable<Tile> meshes)
        {
            meshes.ForEach(RegisterStaticMesh);
        }

        public void RegisterEntity(Entity entity)
        {
            entities.Add(entity);
            CollisionManager.RegisterCollideable(entity);
            PhysicsManager.RegisterPhysicsBody(entity);
            RenderManager.RegisterRenderable(entity);
        }

        public void RegisterEntities(IEnumerable<Entity> entities)
        {
            entities.ForEach(RegisterEntity);
        }

        public IEnumerable<Tile> GetStaticMeshes()
        {
            return staticMeshes;
        }

        public List<LoopShape> CollisionGeometry { get; private set; }
    }
}
