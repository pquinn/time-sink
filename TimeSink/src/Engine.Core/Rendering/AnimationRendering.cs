using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class AnimationRendering : IRendering
    {
        protected string textureKey;
        protected Vector2 position;
        protected float rotation;
        protected Vector2 scale;
        protected Rectangle srcRectangle;
        protected Rectangle destRectangle;

        public Rectangle SrcRectangle
        {
            get { return srcRectangle; }
            set { srcRectangle = value; }
        }

        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public AnimationRendering(string textureKey, Vector2 position, float rotation, Vector2 scale, Rectangle srcRect)
        {
            this.textureKey = textureKey;
            this.position = position;
            this.rotation = rotation;
            this.srcRectangle = srcRect;
            this.scale = scale;
        }

        public void Draw(SpriteBatch spriteBatch, Caching.IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(textureKey);

            var origin = new Vector2(srcRectangle.Width / 2, srcRectangle.Height / 2);

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                globalTransform);

            spriteBatch.Draw(
                texture,
                position,
                srcRectangle,
                Color.White,
                (float)rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            spriteBatch.End();
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(Caching.IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Microsoft.Xna.Framework.Vector2 point, Caching.IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Microsoft.Xna.Framework.Vector2 point, Caching.IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix transform)
        {
            var texture = cache.GetResource(textureKey);
            var center = new Vector2(position.X + srcRectangle.X + srcRectangle.Width / 2,
                                     position.Y + srcRectangle.Y + srcRectangle.Height / 2);

            var debug = Vector2.Transform(center, transform);
            return debug;
        }
    }
}
