using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    public class WireFrameRendering : BasicRendering
    {
        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache)
        {
            base.Draw(spriteBatch, cache);

            var textureToHighlight = cache.GetResource(textureKey);
            var bot = position.X + textureToHighlight.Width;
            var right = position.Y + textureToHighlight.Height;
            var topLeft = new Vector2(position.X, position.Y);
            var topRight = new Vector2(right, position.Y);
            var botLeft = new Vector2(position.X, bot);
            var botRight = new Vector2(right, bot);
            spriteBatch.DrawLine(
                cache.GetResource("blank"),
                topLeft, topRight, 2, Color.Black);
            spriteBatch.DrawLine(
                cache.GetResource("blank"),
                topLeft, botLeft, 2, Color.Black);
            spriteBatch.DrawLine(
                cache.GetResource("blank"),
                botLeft, botRight, 2, Color.Black);
            spriteBatch.DrawLine(
                cache.GetResource("blank"),
                topLeft, botRight, 2, Color.Black);
        } 
    }
}
