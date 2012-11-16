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

        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(textureKey);

            var relativeTransform =
               Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
               Matrix.CreateRotationZ(rotation) *
               Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0));

            var origin = new Vector2(texture.Width / 2, texture.Height / 2);

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
                Vector2.Transform(Vector2.Zero, relativeTransform) + origin,
                srcRectangle,
                tintColor,
                (float)rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            spriteBatch.End();
        }
    }
}
