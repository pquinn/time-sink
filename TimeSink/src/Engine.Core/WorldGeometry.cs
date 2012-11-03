using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using TimeSink.Engine.Core.Collisions;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Rendering;

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
            collisionGeometry.Geometry.Add(new CollisionRectangle(r));
        }

        public void Load(StarterGame game)
        {
            geoTexture = game.Content.Load<BaseRenderableEffect>("Materials/Dude");

            // First create and submit the empty player container.
            geoSprites = game.SpriteManager.CreateSpriteContainer();
            game.SceneInterface.ObjectManager.Submit(geoSprites);
        }

        public void Draw(GameTime gameTime)
        {
            geoSprites.Begin();

            foreach (var geo in collisionGeometry.Geometry)
            {
                if (geo is CollisionRectangle)
                {
                    var rect = geo as CollisionRectangle;
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
        public void OnCollidedWith(UserControlledCharacter character)
        {
            character.GravityEnabled = false;
            character.PhysicsController.Velocity = Vector2.Zero;
        }
    }
}