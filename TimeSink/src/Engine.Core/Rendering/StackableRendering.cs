using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class StackableRendering : IRendering
    {
        Stack<IRendering> renderStack;
        Vector2 parentPosition;
        float parentRotation;
        Vector2 parentScale;

        public StackableRendering(Stack<IRendering> renderingStack)
            : this(renderingStack, Vector2.Zero, 0.0f, Vector2.One)
        { }

        public StackableRendering(Stack<IRendering> renderingStack, Vector2 position, float rotation, Vector2 scale)
        {
            this.renderStack = renderingStack;
            this.parentPosition = position;
            this.parentRotation = rotation;
            this.parentScale = scale;
        }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(parentScale.X, parentScale.Y, 1)) *
                Matrix.CreateRotationZ(parentRotation) *
                Matrix.CreateTranslation(parentPosition.X, parentPosition.Y, 0) *
                globalTransform;

            var centerAcc = Vector2.Zero;
            renderStack.ForEach(r => centerAcc += r.GetCenter(cache, relativeTransform));

            return centerAcc / renderStack.Count;
        }


        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix transform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(parentScale.X, parentScale.Y, 1)) *
                Matrix.CreateRotationZ(parentRotation) *
                Matrix.CreateTranslation(parentPosition.X, parentPosition.Y, 0) *
                transform;

            foreach (var rendering in renderStack)
            {
                rendering.Draw(spriteBatch, cache, relativeTransform);
            }
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(parentScale.X, parentScale.Y, 1)) *
                Matrix.CreateRotationZ(parentRotation) *
                Matrix.CreateTranslation(parentPosition.X, parentPosition.Y, 0) *
                globalTransform;

            return renderStack.Any(r => r.Contains(point, cache, relativeTransform));
        }


        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }
    }
}
