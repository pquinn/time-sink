using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace TimeSink.Engine.Core.Rendering
{
    /// <summary>
    /// Note this a temporary rendering module that simply renders a texture.
    /// This will be heavily modified as we need to add rendering functionality.
    /// </summary>
    public class BasicRendering : IRendering
    {
        protected string textureKey;
        protected Vector2 position;
        protected float rotation;
        protected Vector2 scale;
        protected Rectangle? srcRectangle;
        protected Rectangle? destRectangle;

        public Rectangle? SrcRectangle
        {
            get { return srcRectangle; }
            set { srcRectangle = value; }
        }
        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public BasicRendering(string textureKey)
            : this(textureKey, Vector2.Zero, 0.0f, Vector2.One)
        { }

        public BasicRendering(string textureKey, Vector2 position, float rotation, Vector2 scale)
            : this(textureKey, position, rotation, scale, null)
        { }

        public BasicRendering(string textureKey, Vector2 position, float rotation, Vector2 scale, Rectangle? srcRect)
        {
            this.textureKey = textureKey;
            this.position = position;
            this.rotation = rotation;
            this.srcRectangle = srcRect;
            this.scale = scale;
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache,
            Vector2 positionOffset, float rotationOffset, Vector2 scaleOffset)
        {
            spriteBatch.Draw(
                cache.GetResource(textureKey),
                positionOffset + position,
                srcRectangle,
                Color.White,
                rotationOffset + rotation,
                Vector2.Zero,
                scaleOffset * scale,
                SpriteEffects.None,
                0
            );
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix transform)
        {
            var texture = cache.GetResource(textureKey);

            var relativeTransform =
               Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
               Matrix.CreateRotationZ(rotation) *
               Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0)) *
               transform;

            var origin = Vector2.Transform(
                new Vector2(texture.Width / 2, texture.Height / 2),
                transform);

            if (InputManager.Instance.Pressed(Keys.B))
            {
                //Debugger.Break();
            }

            spriteBatch.Draw(
                texture,
                position + origin,
                srcRectangle,
                Color.White,
                (float)rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(textureKey);

            var relativeTransform =
                Matrix.CreateTranslation(new Vector3(-texture.Width / 2, -texture.Height / 2, 0)) *
                Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(texture.Width / 2, texture.Height / 2, 0)) *
                Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0)) *
                globalTransform;

            var topLeft = Vector2.Transform(Vector2.Zero, relativeTransform);
            var topRight = Vector2.Transform(
                new Vector2(
                    texture.Width, 0),
                relativeTransform);
            var botLeft = Vector2.Transform(
                new Vector2(0, texture.Height),
                relativeTransform);
            var botRight = Vector2.Transform(
                new Vector2(texture.Width, texture.Height),
                relativeTransform);

            if (InputManager.Instance.Pressed(Keys.N))
            {
                Debugger.Break();
            }

            return new NonAxisAlignedBoundingBox(topLeft, topRight, botLeft, botRight);
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
            var texture = cache.GetResource(textureKey);

            var left = position.X + positionOffset.X;
            var right = left + texture.Width;
            var top = position.Y + positionOffset.Y;
            var bot = top + texture.Height;

            return (point.X >= left) && (point.X <= right) &&
                   (point.Y >= top) && (point.Y <= bot);
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix transform)
        {
            var texture = cache.GetResource(textureKey);

            var relativeTransform =
                Matrix.CreateTranslation(new Vector3(-texture.Width / 2, -texture.Height / 2, 0)) *
                Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(texture.Width / 2, texture.Height / 2, 0)) *
                Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0)) *
                transform;

            var pointInRenderCoordinates =
                Vector2.Transform(point, Matrix.Invert(relativeTransform));

            return (pointInRenderCoordinates.X >= 0) &&
                   (pointInRenderCoordinates.X <= texture.Width) &&
                   (pointInRenderCoordinates.Y >= 0) &&
                   (pointInRenderCoordinates.Y <= texture.Height);
        }

        public void GetBoundingBox(IResourceCache<Texture2D> cache, ref BoundingBox acc, Vector2 positionOffset)
        {
            var texture = cache.GetResource(textureKey);
            var relativeLeft = positionOffset.X + position.X;
            var relativeRight = relativeLeft + texture.Width;
            var relativeTop = position.Y - positionOffset.Y;
            var relativeBot = relativeTop + texture.Height;

            acc = new BoundingBox(
                Math.Min(acc.Min_X, relativeLeft),
                Math.Max(acc.Max_X, relativeRight),
                Math.Max(acc.Min_Y, relativeBot),
                Math.Min(acc.Max_Y, relativeTop));
        }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix transform)
        {
            var texture = cache.GetResource(textureKey);
            var center = new Vector2(position.X + texture.Width / 2, position.Y + texture.Height / 2);

            var debug = Vector2.Transform(center, transform);
            return debug;
        }
    }
}
