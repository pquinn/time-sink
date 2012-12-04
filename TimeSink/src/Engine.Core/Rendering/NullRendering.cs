using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    public class NullRendering : IRendering
    {
        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix transform)
        {
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Microsoft.Xna.Framework.Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Microsoft.Xna.Framework.Vector2 point, Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Microsoft.Xna.Framework.Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public Microsoft.Xna.Framework.Vector2 GetCenter(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Microsoft.Xna.Framework.Matrix globalTransform)
        {
            throw new NotImplementedException();
        }
    }
}
