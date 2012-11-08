using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    /// <summary>
    /// Note this a temporary rendering module that simply renders a texture.
    /// This will be heavily modified as we need to add rendering functionality.
    /// </summary>
    public class BasicRendering : IRendering
    {
        protected string textureKey;
        protected Vector2 position;
        protected Rectangle? srcRectangle;

        public BasicRendering(string textureKey)
            : this(textureKey, Vector2.Zero)
        { }

        public BasicRendering(string textureKey, Vector2 position)
            : this(textureKey, position, null)
        { }

        public BasicRendering(string textureKey, Vector2 position, Rectangle? srcRect)
        {
            this.textureKey = textureKey;
            this.position = position;
            this.srcRectangle = srcRect;
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
            spriteBatch.Draw(
                cache.GetResource(textureKey),
                positionOffset + position,
                srcRectangle,
                Color.White
            );
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
            var texture = cache.GetResource(textureKey);
            var left = position.X + positionOffset.X;
            var right = left + texture.Width;
            var top = position.Y + positionOffset.Y;
            var bot = top + texture.Height;

            return (point.X >= left) && (point.X <= right) &&
                   (point.Y >= top) && (point.Y <= bot);
        }

        public void GetBoundingBox(IResourceCache<Texture2D> cache, ref BoundingBox acc, Vector2 positionOffset)
        {
            var texture = cache.GetResource(textureKey);
            var relativeLeft = positionOffset.X + position.X;
            var relativeRight = relativeLeft + texture.Width;
            var relativeTop = position.Y - positionOffset.Y;
            var relativeBot = relativeTop + texture.Height;

            acc = new BoundingBox(
                Math.Min(acc.Min_X, relativeLeft),
                Math.Max(acc.Max_X, relativeRight),
                Math.Max(acc.Min_Y, relativeBot),
                Math.Min(acc.Max_Y, relativeTop));
        }
    }
}
