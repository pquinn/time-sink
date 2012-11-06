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

        public BasicRendering(string textureKey, Vector2 position)
        {
            this.textureKey = textureKey;
            this.position = position;
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache)
        {
            spriteBatch.Draw(cache.GetResource(textureKey), position, Color.White);
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Rectangle sourceRect)
        {
            spriteBatch.Draw(cache.GetResource(textureKey), position, sourceRect, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
        }

        public virtual void DrawSelected(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Color color)
        {
                var texture = cache.GetResource(textureKey);
                var blank = cache.GetResource("blank");
                var right = position.X + texture.Width;
                var bot = position.Y + texture.Height;
                var topLeft = new Vector2(position.X, position.Y);
                var botRight = new Vector2(right, bot);
                spriteBatch.DrawRect(
                    blank,
                    topLeft, botRight, 5, color);
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache)
        {
            var texture = cache.GetResource(textureKey);
            var right = position.X + texture.Width;
            var bot = position.Y + texture.Height;

            return (point.X > position.X) && (point.X < right) &&
                   (point.Y > position.Y) && (point.Y < bot);
        }
    }
}
