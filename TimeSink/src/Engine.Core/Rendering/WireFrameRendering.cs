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
            : base(textureKey, position, 0f, Vector2.One)
        { }

        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, 
            Vector2 positionOffset, float rotationOffset, Vector2 scaleOffset)
        {
            base.Draw(spriteBatch, cache, positionOffset, rotationOffset, scaleOffset);

            var topLeft = positionOffset + position;

            var trans = Matrix.CreateScale(new Vector3(scaleOffset + scale, 0))
                * Matrix.CreateRotationZ(rotationOffset + rotation)
                * Matrix.CreateTranslation(new Vector3(topLeft, 0));

            var textureToHighlight = cache.GetResource(textureKey);
            
            var topRight = Vector2.Transform(new Vector2(textureToHighlight.Width, 0), trans);
            var botLeft = Vector2.Transform(new Vector2(0, textureToHighlight.Height), trans);
            var botRight = Vector2.Transform(new Vector2(textureToHighlight.Width, textureToHighlight.Height), trans);
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
