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
        RenderLayer RenderLayer { get; set; }

        float DepthWithinLayer { get; set; }

        void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix transform);

        NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform);

        bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix globalTransform);

        Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix globalTransform);
    }
}
