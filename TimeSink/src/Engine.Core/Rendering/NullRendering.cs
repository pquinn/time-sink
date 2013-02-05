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
        public RenderLayer RenderLayer { get; set; }
        public float DepthWithinLayer { get; set; }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix transform)
        {
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Microsoft.Xna.Framework.Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Microsoft.Xna.Framework.Vector2 point, Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Microsoft.Xna.Framework.Matrix globalTransform)
        {
            return false;
        }

        public Microsoft.Xna.Framework.Vector2 GetCenter(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Microsoft.Xna.Framework.Matrix globalTransform)
        {
            throw new NotImplementedException();
        }
    }
}
