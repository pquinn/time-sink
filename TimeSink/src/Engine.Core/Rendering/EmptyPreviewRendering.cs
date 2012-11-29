using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core.Input;

namespace TimeSink.Engine.Core.Rendering
{
    public class EmptyPreviewRendering : BasicRendering
    {
        public EmptyPreviewRendering(Vector2 position, int rot, int width, int height)
            : base("blank", position, rot, new Vector2(width, height))
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(textureKey);

            Vector2 origin = new Vector2(scale.X, scale.Y);

            if (InputManager.Instance.Pressed(Keys.B))
            {
                //Debugger.Break();
            }

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
    }
}
