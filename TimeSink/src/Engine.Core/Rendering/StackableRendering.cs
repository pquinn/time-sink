using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class StackableRendering : IRendering
    {
        Stack<Tuple<string, Vector2>> textureKeysAndRelativePositions;
        Vector2 parentPosition;

        public StackableRendering(Stack<Tuple<string, Vector2>> textureKeysAndRelativePositions, Vector2 parentPosition)
        {
            this.textureKeysAndRelativePositions = textureKeysAndRelativePositions;
            this.parentPosition = parentPosition;
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache)
        {
            foreach (var textureKey in textureKeysAndRelativePositions)
            {
                spriteBatch.Draw(
                    cache.GetResource(textureKey.Item1), 
                    parentPosition + textureKey.Item2,
                    Color.White);
            }
        }
    }
}
