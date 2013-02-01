using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class TextRendering : IRendering
    {
        protected String text;
        Vector2 parentPosition;
        float parentRotation;
        Vector2 parentScale;
        Color color;

        public RenderLayer RenderLayer { get; set; }

        public TextRendering(String text, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            this.text = text;
            this.parentPosition = position;
            this.parentRotation = rotation;
            this.parentScale = scale;
            this.color = color;
        }
        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(parentScale.X, parentScale.Y, 1)) *
                Matrix.CreateRotationZ(parentRotation) *
                Matrix.CreateTranslation(parentPosition.X, parentPosition.Y, 0) *
                globalTransform;


            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                Matrix.Identity);




            spriteBatch.DrawString(EngineGame.Instance.ScreenManager.Font, text, parentPosition, color);
            spriteBatch.End();
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix transform)
        {
            return false;
        }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix transform)
        {
            return Vector2.One;
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            return new NonAxisAlignedBoundingBox();
        }
    }
}
