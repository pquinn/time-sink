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
        string textureKey;
        Vector2 position;

        public BasicRendering(string textureKey, Vector2 position)
        {
            this.textureKey = textureKey;
            this.position = position;
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache)
        {
            spriteBatch.Draw(cache.GetResource(textureKey), position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Rectangle sourceRect)
        {
            spriteBatch.Draw(cache.GetResource(textureKey), position, sourceRect, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
        }

    }
}
