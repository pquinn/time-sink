using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    public interface IRendering
    {
        void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, 
            Vector2 positionOffset, float rotationOffset, Vector2 scaleOffset);
    }
}
