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
        public WireFrameRendering(string textureKey, Vector2 position)
            : base(textureKey, position)
        { }

        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
<<<<<<< HEAD
            base.Draw(spriteBatch, cache);
=======
            base.Draw(spriteBatch, cache, positionOffset);

            var textureToHighlight = cache.GetResource(textureKey);
            var bot = position.X + textureToHighlight.Width;
            var right = position.Y + textureToHighlight.Height;
            var topLeft = new Vector2(position.X, position.Y) + positionOffset;
            var topRight = new Vector2(right, position.Y) + positionOffset;
            var botLeft = new Vector2(position.X, bot) + positionOffset;
            var botRight = new Vector2(right, bot) + positionOffset;
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
>>>>>>> d21c01f52a900f3db5c874a32f3238a62875d517
        } 
    }
}
