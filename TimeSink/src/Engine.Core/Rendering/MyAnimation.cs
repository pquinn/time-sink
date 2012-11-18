using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class MyAnimation : IRendering 
    {
        private List<MyAnimation> children;
        private Vector2 relativePos;
        private float relativeRot;
        private Vector2 relativeScale;
        private BasicRendering frameRender;

        public MyAnimation(string texture, Rectangle? srcRect, Vector2 relativePos, float relativeRot, Vector2 relativeScale, List<MyAnimation> children)
        {
            this.children = children;
            this.relativePos = relativePos;
            this.relativeScale = relativeScale;
            this.relativeRot = relativeRot;
            this.frameRender = new BasicRendering(texture, relativePos, relativeRot, relativeScale, srcRect);
        }

        public void Draw(SpriteBatch spriteBatch, Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Matrix transform)
        {
            var relativeMatrix =
                Matrix.CreateScale(new Vector3(relativeScale, 0)) *
                Matrix.CreateRotationZ(relativeRot) *
                Matrix.CreateTranslation(new Vector3(relativePos, 1)) *
                transform;

            frameRender.Draw(spriteBatch, cache, transform);

            foreach (var child in children)
            {
                child.Draw(spriteBatch, cache, relativeMatrix);
            }
        }

        #region Not Impl

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Vector2 positionOffset, float rotationOffset, Vector2 scaleOffset)
        {
            throw new NotImplementedException();
        }

        public void GetBoundingBox(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, ref BoundingBox acc, Vector2 positionOffset)
        {
            throw new NotImplementedException();
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Vector2 point, Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Vector2 positionOffset)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Vector2 point, Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public Vector2 GetCenter(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
