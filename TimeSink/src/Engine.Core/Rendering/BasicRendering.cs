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
        #region fields

        protected Rectangle? destRectangle;

        #endregion

        #region properties

        public RenderLayer RenderLayer { get; set; }
        public String TextureKey { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Size { get; set; }       
        public Rectangle? SrcRectangle { get; set; }
        public Color TintColor { get; set; }

        private float depthClamp;
        public float DepthWithinLayer { get; set; }

        #endregion

        public BasicRendering(string textureKey)
        {
            this.TextureKey = textureKey;
            this.Position = Vector2.Zero;
            this.Rotation = 0.0f;
            this.Scale = Vector2.One;
            this.Size = Vector2.Zero;
            this.RenderLayer = Rendering.RenderLayer.Gameground;
            this.DepthWithinLayer = 0;
            this.TintColor = Color.White;
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(TextureKey);

            Vector2 origin;
            if (SrcRectangle.HasValue)
                origin = new Vector2(SrcRectangle.Value.Width / 2, SrcRectangle.Value.Height / 2);
            else
                origin = new Vector2(texture.Width / 2, texture.Height / 2);

            var scale = Size / new Vector2(texture.Width, texture.Height);
            if (scale == Vector2.Zero)
                scale = Vector2.One;
            scale *= Scale;

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
                Position,
                SrcRectangle,
                TintColor,
                Rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            spriteBatch.End();
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var texture = cache.GetResource(TextureKey);

            var scale = (Size != Vector2.Zero) ? Size * Scale : Scale;
            var relativeTransform =
                Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0)) *
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
            var texture = cache.GetResource(TextureKey);

            var scale = (Size != Vector2.Zero) ? Size * Scale : Scale;
            var relativeTransform =
                Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0)) *
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
            var texture = cache.GetResource(TextureKey);

            return Vector2.Transform(Position, transform);
        }
    }
}
