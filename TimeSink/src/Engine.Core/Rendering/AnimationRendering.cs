using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Input;

namespace TimeSink.Engine.Core.Rendering
{
    public class AnimationRendering : IRendering
    {
        protected string textureKey;
        protected Vector2 position;
        protected float rotation;
        protected Vector2 scale;
        protected Rectangle srcRectangle;
        protected Rectangle destRectangle;

        public Rectangle SrcRectangle
        {
            get { return srcRectangle; }
            set { srcRectangle = value; }
        }
        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public AnimationRendering(string textureKey, Vector2 position, float rotation, Vector2 scale, Rectangle srcRect)
        {
            this.textureKey = textureKey;
            this.position = position;
            this.rotation = rotation;
            this.srcRectangle = srcRect;
            this.scale = scale;
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Microsoft.Xna.Framework.Vector2 positionOffset, float rotationOffset, Microsoft.Xna.Framework.Vector2 scaleOffset)
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch, Caching.IResourceCache<Texture2D> cache, Matrix transform)
        {
            var texture = cache.GetResource(textureKey);

            var relativeTransform =
               Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
               Matrix.CreateRotationZ(rotation) *
               Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0)) *
               transform;

            var origin = new Vector2(texture.Width / 2, texture.Height / 2);

            if (InputManager.Instance.Pressed(Keys.B))
            {
                //Debugger.Break();
            }

            spriteBatch.Draw(
                texture,
                Vector2.Transform(Vector2.Zero, relativeTransform) + origin,
                srcRectangle,
                Color.White,
                (float)rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
        }

        public void GetBoundingBox(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, ref BoundingBox acc, Microsoft.Xna.Framework.Vector2 positionOffset)
        {
            var texture = cache.GetResource(textureKey);
            var relativeLeft = positionOffset.X + position.X;
            var relativeRight = relativeLeft + srcRectangle.X + srcRectangle.Width;
            var relativeTop = position.Y - positionOffset.Y;
            var relativeBot = relativeTop + srcRectangle.Y + srcRectangle.Height;

            acc = new BoundingBox(
                Math.Min(acc.Min_X, relativeLeft),
                Math.Max(acc.Max_X, relativeRight),
                Math.Max(acc.Min_Y, relativeBot),
                Math.Min(acc.Max_Y, relativeTop));
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(Caching.IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Microsoft.Xna.Framework.Vector2 point, Caching.IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Microsoft.Xna.Framework.Vector2 point, Caching.IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public Microsoft.Xna.Framework.Vector2 GetCenter(Caching.IResourceCache<Texture2D> cache, Matrix transform)
        {
            var texture = cache.GetResource(textureKey);
            var center = new Vector2(position.X + srcRectangle.X + srcRectangle.Width / 2,
                                     position.Y + srcRectangle.Y + srcRectangle.Height / 2);

            var debug = Vector2.Transform(center, transform);
            return debug;
        }
    }
}
