using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        public WorldGeometry(Rectangle r)
        {
            collisionGeometry.Geometry.Add(new AACollisionRectangle(r));
        }

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
                if (geo is AACollisionRectangle)
                {
                    var rect = geo as AACollisionRectangle;
                 /*   geoSprites.Add(
                        geoTexture,
                        new Vector2(2f, .32f),
                        new Vector2(rect.Rect.Left, rect.Rect.Top),
                        0);*/
                    spriteBatch.Draw(geoTexture, rect.Rect, Color.White);
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