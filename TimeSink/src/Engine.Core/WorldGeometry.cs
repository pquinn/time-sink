using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using TimeSink.Engine.Core.Collisions;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Rendering;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Core;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Engine.Core.Collisions
{
    public class WorldGeometry : ICollideable
    {
        private CollisionSet collisionGeometry = new CollisionSet();
        private BaseRenderableEffect geoTexture;
        private SpriteContainer geoSprites;
        public ICollisionGeometry CollisionGeometry
        {
            get { return collisionGeometry; }
        }

        public WorldGeometry(Rectangle r)
        {
            collisionGeometry.Geometry.Add(new AACollisionRectangle(r));
        }

        public void Load(ContentManager content, SpriteManager manager, SceneInterface scene)
        {
            geoTexture = content.Load<BaseRenderableEffect>("Materials/Dude");

            // First create and submit the empty player container.
            geoSprites = manager.CreateSpriteContainer();
            scene.ObjectManager.Submit(geoSprites);
        }

        public void Draw(GameTime gameTime)
        {
            geoSprites.Begin();

            foreach (var geo in collisionGeometry.Geometry)
            {
                if (geo is AACollisionRectangle)
                {
                    var rect = (AACollisionRectangle)geo;
                    geoSprites.Add(
                        geoTexture,
                        new Vector2(2f, .32f),
                        new Vector2(rect.Rect.Left, rect.Rect.Top),
                        0);
                }
            }

            geoSprites.End();
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(IPhysicsEnabledBody body, CollisionInfo info)
        {
            body.PhysicsController.Position += info.MinimumTranslationVector;
        }
    }
}