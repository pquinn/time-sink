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
    public class PivotedRendering : IRendering
    {
        protected string textureKey;
        protected Vector2 position;
        protected Vector2 pivotPosition;
        protected float rotation;
        protected Vector2 scale;
        protected Rectangle? srcRectangle;
        protected Rectangle? destRectangle;

        public RenderLayer RenderLayer { get; set; }
        public float DepthWithinLayer { get; set; }

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

        public PivotedRendering(string textureKey)
            : this(textureKey, Vector2.Zero, 0.0f, Vector2.One)
        { }

        public PivotedRendering(string textureKey, Vector2 position, float rotation, Vector2 scale)
            : this(textureKey, position, rotation, scale, null)
        { }

        public PivotedRendering(string textureKey, Vector2 position, float rotation, Vector2 scale, Rectangle? srcRect)
            : this (textureKey, position, position, rotation, scale, srcRect)
        {
            this.textureKey = textureKey;
            this.position = position;
            this.rotation = rotation;
            this.srcRectangle = srcRect;
            this.scale = scale;
        }

        public PivotedRendering(string textureKey, Vector2 position, Vector2 pivotPosition, float rotation, Vector2 scale, Rectangle? srcRect)
        {
            this.textureKey = textureKey;
            this.position = position;
            this.pivotPosition = pivotPosition;
            this.rotation = rotation;
            this.srcRectangle = srcRect;
            this.scale = scale;
            this.DepthWithinLayer = .5f;
            this.RenderLayer = Rendering.RenderLayer.Gameground;
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(textureKey);

            Vector2 origin;
            if (srcRectangle.HasValue)
                origin = new Vector2(srcRectangle.Value.Width / 2, srcRectangle.Value.Height / 2);
            else
                origin = new Vector2(texture.Width / 2, 0);
                //origin = new Vector2(texture.Width / 2, texture.Height / 2);

            if (InputManager.Instance.Pressed(Keys.B))
            {
                //Debugger.Break();
            }

            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                globalTransform);

            spriteBatch.Draw(
                texture,
                position - new Vector2(0, texture.Height/4),
                srcRectangle,
                Color.White,
                (float)rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            spriteBatch.End();
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(textureKey);

            var relativeTransform =
                Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0)) *
                globalTransform;

            var topLeft = Vector2.Transform(
                new Vector2(-texture.Width / 2, -texture.Height / 2),
                relativeTransform);
            var topRight = Vector2.Transform(
                new Vector2(texture.Width / 2, -texture.Height / 2),
                relativeTransform);
            var botLeft = Vector2.Transform(
                new Vector2(-texture.Width / 2, texture.Height / 2),
                relativeTransform);
            var botRight = Vector2.Transform(
                new Vector2(texture.Width / 2, texture.Height / 2),
                relativeTransform);

            if (InputManager.Instance.Pressed(Keys.N))
            {
                Debugger.Break();
            }

            return new NonAxisAlignedBoundingBox(topLeft, topRight, botLeft, botRight);
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix transform)
        {
            var texture = cache.GetResource(textureKey);

            var relativeTransform =
                Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0)) *
                transform;

            var pointInRenderCoordinates =
                Vector2.Transform(point, Matrix.Invert(relativeTransform));

            return (pointInRenderCoordinates.X >= -texture.Width / 2) &&
                   (pointInRenderCoordinates.X <= texture.Width / 2) &&
                   (pointInRenderCoordinates.Y >= -texture.Height / 2) &&
                   (pointInRenderCoordinates.Y <= texture.Height / 2);
        }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix transform)
        {
            var texture = cache.GetResource(textureKey);

            return Vector2.Transform(position, transform);
        }
    }
}
