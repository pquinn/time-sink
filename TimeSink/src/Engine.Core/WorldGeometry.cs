using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core.Collisions
{
    public class WorldGeometry : Entity
    {
        const string WORLD_TEXTURE_NAME = "Textures/giroux";

        private CollisionSet collisionGeometry = new CollisionSet();
        private Texture2D geoTexture;
        private SpriteBatch geoSprites;
        public override ICollisionGeometry CollisionGeometry
        {
            get { return collisionGeometry; }
        }

        public HashSet<ICollisionGeometry> CollisionSet
        {
            get { return collisionGeometry.Geometry; }
        }

        public WorldGeometry() { }

        public override void Load(EngineGame game)
        {
            game.TextureCache.LoadResource(WORLD_TEXTURE_NAME);

            // First create and submit the empty player container.
          /*  geoSprites = manager.CreateSpriteContainer();
            scene.ObjectManager.Submit(geoSprites);*/
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(ICollideable body, CollisionInfo info)
        {
            if (body is IPhysicsEnabledBody)
            {
                var phys = (body as IPhysicsEnabledBody).PhysicsController;
                phys.Position -= info.MinimumTranslationVector + new Vector2(0, -1);
            }
        }

        public override IRendering Rendering
        {
            get 
            {
                var renderStack = new Stack<IRendering>();

                foreach (var geo in collisionGeometry.Geometry)
                {
                    if (geo is CollisionRectangle)
                    {
                        var rect = (CollisionRectangle)geo;

                        renderStack.Push(new RectangleRendering(
                            WORLD_TEXTURE_NAME,
                            new Rectangle(
                                (int)rect.TopLeft.X, 
                                (int)rect.TopLeft.Y,
                                (int)(rect.TopRight.X - rect.TopLeft.X), 
                                (int)(rect.BottomLeft.Y - rect.TopLeft.Y)
                            )
                        ));
                    }
                }

                return new StackableRendering(
                    renderStack
                ); 
            }
        }

        private class RectangleRendering : BasicRendering
        {
            Rectangle destinationRect;

            public RectangleRendering(string key, Rectangle dest) 
                : base(key)
            {
                destinationRect = dest;
            }

            public override void Draw(SpriteBatch spriteBatch, Caching.IResourceCache<Texture2D> cache, Vector2 positionOffset)
            {
                spriteBatch.Draw(
                    cache.GetResource(textureKey),
                    destinationRect,
                    Color.White
                );
            }
        }

        public override IPhysicsParticle PhysicsController
        {
            get { throw new NotImplementedException(); }
        }

        public override void HandleKeyboardInput(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}