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
        Stack<Tuple<string, Vector2>> textureKeysAndRelativePositions;
        Vector2 parentPosition;

        public StackableRendering(Stack<Tuple<string, Vector2>> textureKeysAndRelativePositions, Vector2 parentPosition)
        {
            this.textureKeysAndRelativePositions = textureKeysAndRelativePositions;
            this.parentPosition = parentPosition;
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache)
        {
            foreach (var textureKey in textureKeysAndRelativePositions)
            {
                spriteBatch.Draw(
                    cache.GetResource(textureKey.Item1), 
                    parentPosition + textureKey.Item2,
                    Color.White);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Rectangle sourceRect)
        {
            throw new NotImplementedException();
        }

        public void DrawSelected(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Color color)
        {
            var blank = cache.GetResource("blank");

            var rect = GetFullRectangle(cache);

            spriteBatch.DrawRect(blank, rect, 5, color);
        }

        public virtual bool Contains(Vector2 point, IResourceCache<Texture2D> cache)
        {
            return textureKeysAndRelativePositions.Any(
                pair => 
                    {
                        var texture = cache.GetResource(pair.Item1);
                        var relativeLeft = parentPosition.X + pair.Item2.X;
                        var relativeRight = relativeLeft + texture.Width;
                        var relativeTop = parentPosition.Y - pair.Item2.Y;
                        var relativeBot = relativeTop - texture.Height; 
   
                        return (point.X > relativeLeft) && (point.X < relativeRight) &&
                               (point.Y > relativeTop) && (point.Y < relativeBot);
                    });
        }

        private Rectangle GetFullRectangle(IResourceCache<Texture2D> cache)
        {
            var top = Single.PositiveInfinity;
            var left = Single.PositiveInfinity;
            var right = 0f;
            var bot = 0f;

            foreach (var pair in textureKeysAndRelativePositions)
            {
                var texture = cache.GetResource(pair.Item1);
                var relativeLeft = parentPosition.X + pair.Item2.X;
                var relativeRight = relativeLeft + texture.Width;
                var relativeTop = parentPosition.Y - pair.Item2.Y;
                var relativeBot = relativeTop - texture.Height;

                left = Math.Min(left, relativeLeft);
                right = Math.Max(right, relativeRight);
                bot = Math.Max(bot, relativeBot);
                top = Math.Min(top, relativeTop);
            }

            return new Rectangle(
                (int)left, 
                (int)right, 
                (int)(right - left), 
                (int)(bot - top));
        }
    }
}
