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
    public class SizedRendering : BasicRendering
    {
        public SizedRendering(string texture, Vector2 position, int rot, int width, int height)
            : base(texture, position, rot, Vector2.One)
        {
            Size = new Vector2(width, height);
        }

        public Vector2 Size { get; set; }

        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(textureKey);

            var origin = new Vector2(texture.Width / 2, texture.Height / 2);
            var newScale = Size / new Vector2(texture.Width, texture.Height);

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
                newScale,
                SpriteEffects.None,
                0
            );

            spriteBatch.End();
        }
    }
}
