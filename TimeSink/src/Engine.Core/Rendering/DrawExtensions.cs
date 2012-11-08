using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    public static class DrawExstensions
    {
        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, int thinkness, Color color)
        {
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            float length = Vector2.Distance(start, end);

            spriteBatch.Draw(texture, start, null, color,
                       angle, Vector2.Zero, new Vector2(length, thinkness),
                       SpriteEffects.None, 0);    
        }

        public static void DrawRect(this SpriteBatch spriteBatch, Texture2D texture, Vector2 topLeft, Vector2 botRight, int thinkness, Color color)
        {
            var botLeft = new Vector2(topLeft.X, botRight.Y);
            var topRight = new Vector2(botRight.X, topLeft.Y);

            spriteBatch.DrawLine(
                texture, 
                topLeft, topRight, 
                thinkness, color);
            spriteBatch.DrawLine(
                texture,
                topLeft, botLeft,
                thinkness, color);
            spriteBatch.DrawLine(
                texture,
                botLeft, botRight,
                thinkness, color);
            spriteBatch.DrawLine(
                texture,
                topRight, botRight,
                thinkness, color);
        }

        public static void DrawRect(this SpriteBatch spriteBatch, Texture2D texture, Rectangle rect, int thinkness, Color color)
        {
            DrawRect(
                spriteBatch, texture, 
                new Vector2(rect.Left, rect.Top), 
                new Vector2(rect.Right, rect.Bottom), 
                thinkness, color);
        }
    }
}
