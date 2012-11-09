using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class TintedRendering : BasicRendering
    {
        private Color tintColor;

        public TintedRendering(string textureKey, Vector2 position, float rotation, Vector2 scale, Color tintColor)
            : base(textureKey, position, rotation, scale, null)
        {
            this.tintColor = tintColor;
        }

        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, 
            Vector2 positionOffset, float rotationOffset, Vector2 scaleOffset)
        {
            spriteBatch.Draw(
                cache.GetResource(textureKey),
                positionOffset + position,
                srcRectangle,
                tintColor,
                rotationOffset + rotation,
                Vector2.Zero,
                scaleOffset * scale,
                SpriteEffects.None,
                0
            );
        }
    }
}
