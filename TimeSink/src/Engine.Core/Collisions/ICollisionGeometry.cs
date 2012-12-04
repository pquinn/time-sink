using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public interface ICollisionGeometry
    {
        void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform);
    }
}
