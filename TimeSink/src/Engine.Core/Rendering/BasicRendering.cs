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
        protected float rotation;
        protected Vector2 scale;
        protected Rectangle? srcRectangle;
        protected Rectangle? destRectangle;

        public BasicRendering(string textureKey)
            : this(textureKey, Vector2.Zero, 0.0f, Vector2.One)
        { }

        public BasicRendering(string textureKey, Vector2 position, float rotation, Vector2 scale)
            : this(textureKey, position, rotation, scale, null)
        { }

        public BasicRendering(string textureKey, Vector2 position, float rotation, Vector2 scale, Rectangle? srcRect)
        {
            this.textureKey = textureKey;
            this.position = position;
            this.rotation = rotation;
            this.srcRectangle = srcRect;
            this.scale = scale;
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache,
            Vector2 positionOffset, float rotationOffset, Vector2 scaleOffset)
        {
            spriteBatch.Draw(
                cache.GetResource(textureKey),
                positionOffset + position,
                srcRectangle,
                Color.White,
                rotationOffset + rotation,
                Vector2.Zero,
                scaleOffset * scale,
                SpriteEffects.None,
                0
            );
        }

        public virtual void DrawSelected(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Color color, BoundingBox acc)
        {
                
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
            var texture = cache.GetResource(textureKey);
            var left = position.X + positionOffset.X;
            var right = left + texture.Width;
            var top = position.Y + positionOffset.Y;
            var bot = top + texture.Height;

            return (point.X > left) && (point.X < right) &&
                   (point.Y > top) && (point.Y < bot);
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
