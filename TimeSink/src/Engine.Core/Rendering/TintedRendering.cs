﻿using System;
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
            :base(textureKey)
        {
            this.tintColor = tintColor;
        }
        public TintedRendering(string textureKey, Vector2 position, float rotation, Vector2 scale, Color tintColor, Rectangle? rect)
            : base(textureKey)
        {
            this.tintColor = tintColor;
        }
        public Color TintColor { get { return tintColor; } set { tintColor = value; } }

        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(TextureKey);

            Vector2 origin;
            if (SrcRectangle.HasValue)
                origin = new Vector2(SrcRectangle.Value.Width / 2, SrcRectangle.Value.Height / 2);
            else
                origin = new Vector2(texture.Width / 2, texture.Height / 2);

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
                Position,
                SrcRectangle,
                tintColor,
                Rotation,
                origin,
                Scale,
                SpriteEffects.None,
                0
            );

            spriteBatch.End();
        }
    }
}
