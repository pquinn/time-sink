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
    }
}
