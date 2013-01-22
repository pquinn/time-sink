using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class TextRendering : IRendering
    {
        protected Vector2 position;
        protected float rotation;
        protected Vector2 scale;
        protected String text;

        public TextRendering(String text, Vector2 position, float rotation, Vector2 scale)
        {
            this.text = text;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                globalTransform);




            spriteBatch.DrawString(EngineGame.Instance.ScreenManager.Font, text, position, Color.Green);
            spriteBatch.End();
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix transform)
        {
            return false;
        }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix transform)
        {
            return Vector2.One;
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            return new NonAxisAlignedBoundingBox();
        }
    }
}
