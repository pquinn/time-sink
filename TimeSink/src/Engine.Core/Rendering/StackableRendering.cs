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

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache,
            Vector2 positionOffset, float rotationOffset, Vector2 scaleOffset)
        {
            foreach (var rendering in renderStack)
            {
                rendering.Draw(
                    spriteBatch, 
                    cache, 
                    positionOffset + parentPosition, 
                    rotationOffset + parentRotation,
                    scaleOffset * parentScale
                );
            }
        }

        //public void DrawSelected(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Color color, BoundingBox acc)
        //{
        //    var blank = cache.GetResource("blank");

        //    var rect = GetFullRectangle(cache);

        //    spriteBatch.DrawRect(blank, rect, 5, color);
        //}

        public virtual bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
            return renderStack.Any(x => x.Contains(point, cache, positionOffset));
            //return textureKeysAndRelativePositions.Any(
            //    pair => 
            //        {
            //            var texture = cache.GetResource(pair.Item1);
            //            var relativeLeft = parentPosition.X + pair.Item2.X;
            //            var relativeRight = relativeLeft + texture.Width;
            //            var relativeTop = parentPosition.Y - pair.Item2.Y;
            //            var relativeBot = relativeTop - texture.Height; 
   
            //            return (point.X > relativeLeft) && (point.X < relativeRight) &&
            //                   (point.Y > relativeTop) && (point.Y < relativeBot);
            //        });
        }

        public void GetBoundingBox(IResourceCache<Texture2D> cache, ref BoundingBox acc, Vector2 positionOffset)
        {
            var newAcc = new BoundingBox(
                      Single.PositiveInfinity, Single.PositiveInfinity,
                      0f, 0f);

            foreach (var rendering in renderStack)
            {
                rendering.GetBoundingBox(cache, ref newAcc, positionOffset + parentPosition);
            }
        }
    }
}
