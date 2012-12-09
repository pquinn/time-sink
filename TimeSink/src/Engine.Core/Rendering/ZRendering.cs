using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Input;

namespace TimeSink.Engine.Core.Rendering
{
  public  class ZRendering : SizedRendering
    {
        public float Depth { get; set; }
        public ZRendering(string texture, Vector2 position, int rot, int width, int height, float depth)
            : base(texture, position, rot, width, height)
        {
            Depth = depth;
        }

        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(textureKey);

            var origin = new Vector2(texture.Width / 2, texture.Height / 2);
            var newScale = Size / new Vector2(texture.Width, texture.Height);


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
                newScale,
                SpriteEffects.None,
                Depth
            );

            spriteBatch.End();
        }
    }
}
