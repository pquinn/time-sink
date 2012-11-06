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
        {
        }

        public override void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache)
        {
            base.Draw(spriteBatch, cache);
        } 
    }
}
