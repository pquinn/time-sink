using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core
{
    public class Level
    {
        private List<StaticMesh> staticMeshes;
        private List<Entity> entities;

        public Level(CollisionManager collisionsManager, PhysicsManager physicsManager, RenderManager renderManager)
        {
            staticMeshes = new List<StaticMesh>();
            entities = new List<Entity>();
            CollisionManager = collisionsManager;
            PhysicsManager = physicsManager;
            RenderManager = renderManager;
        }

        public CollisionManager CollisionManager { get; private set; }

        public PhysicsManager PhysicsManager { get; private set; }

        public RenderManager RenderManager { get; private set; }

        public void RegisterStaticMesh(StaticMesh mesh)
        {
            staticMeshes.Add(mesh);
            CollisionManager.RegisterCollisionBody(mesh);
            PhysicsManager.RegisterPhysicsBody(mesh);
            RenderManager.RegisterRenderable(mesh);
        }

        public void RegisterStaticMeshes(IEnumerable<StaticMesh> meshes)
        {
            meshes.ForEach(RegisterStaticMesh);
        }

        public void RegisterEntity(Entity entity)
        {
            entities.Add(entity);
            CollisionManager.RegisterCollisionBody(entity);
            PhysicsManager.RegisterPhysicsBody(entity);
            RenderManager.RegisterRenderable(entity);
        }

        public void RegisterEntities(IEnumerable<Entity> entities)
        {
            entities.ForEach(RegisterEntity);
        }
    }
}
