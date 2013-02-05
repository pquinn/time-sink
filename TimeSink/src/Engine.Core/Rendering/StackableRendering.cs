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
        public RenderLayer RenderLayer { get; set; }
        public float DepthWithinLayer { get; set; }

        public StackableRendering(Stack<IRendering> renderingStack)
            : this(renderingStack, Vector2.Zero, 0.0f, Vector2.One)
        { }

        public StackableRendering(Stack<IRendering> renderingStack, Vector2 position, float rotation, Vector2 scale)
        {
            RenderStack = renderingStack;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Vector2 Position { get; set; }

        public float Rotation { get; set; }

        public Vector2 Scale { get; set; }

        public Stack<IRendering> RenderStack { get; set; }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(Position.X, Position.Y, 0) *
                globalTransform;

            var centerAcc = Vector2.Zero;
            RenderStack.ForEach(r => centerAcc += r.GetCenter(cache, relativeTransform));

            return centerAcc / RenderStack.Count;
        }


        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix transform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(Position.X, Position.Y, 0) *
                transform;

            foreach (var rendering in RenderStack)
            {
                rendering.Draw(spriteBatch, cache, relativeTransform);
            }
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(Position.X, Position.Y, 0) *
                globalTransform;

            return RenderStack.Any(r => r.Contains(point, cache, relativeTransform));
        }


        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }
    }
}
