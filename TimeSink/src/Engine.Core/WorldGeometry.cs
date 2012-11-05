using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Engine.Core.Collisions
{
    public class WorldGeometry : ICollideable
    {
        private CollisionSet collisionGeometry = new CollisionSet();
        private Texture2D geoTexture;
        private SpriteBatch geoSprites;
        public ICollisionGeometry CollisionGeometry
        {
            get { return collisionGeometry; }
        }

        public HashSet<ICollisionGeometry> CollisionSet
        {
            get { return collisionGeometry.Geometry; }
        }

        public WorldGeometry() { }

        public void Load(ContentManager content /*SpriteManager manager, SceneInterface scene*/)
        {
            geoTexture = content.Load<Texture2D>("Textures/giroux");

            // First create and submit the empty player container.
          /*  geoSprites = manager.CreateSpriteContainer();
            scene.ObjectManager.Submit(geoSprites);*/
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            geoSprites = spriteBatch;
            geoSprites.Begin();

            foreach (var geo in collisionGeometry.Geometry)
            {
                if (geo is CollisionRectangle)
                {

                    var rect = (CollisionRectangle)geo;
                 /*   geoSprites.Add(
                        geoTexture,
                        new Vector2(2f, .32f),
                        new Vector2(rect.Rect.Left, rect.Rect.Top),
                        0);*/
                    spriteBatch.Draw(
                        geoTexture,
                        new Rectangle(
                            (int)rect.TopLeft.X, 
                            (int)rect.TopLeft.Y,
                            (int)(rect.TopRight.X - rect.TopLeft.X), 
                            (int)(rect.BottomLeft.Y - rect.TopLeft.Y)
                        ), 
                        Color.White
                    );
                }
            }

            geoSprites.End();
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
    }
}