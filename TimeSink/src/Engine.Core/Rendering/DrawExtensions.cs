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

        public static void DrawRect(this SpriteBatch spriteBatch, Texture2D texture, Vector2 topLeft, Vector2 botRight, int thickness, Color color)
        {
            var botLeft = new Vector2(topLeft.X, botRight.Y);
            var topRight = new Vector2(botRight.X, topLeft.Y);

            spriteBatch.DrawLine(
                texture, 
                topLeft, topRight, 
                thickness, color);
            spriteBatch.DrawLine(
                texture,
                topLeft, botLeft,
                thickness, color);
            spriteBatch.DrawLine(
                texture,
                botLeft, botRight,
                thickness, color);
            spriteBatch.DrawLine(
                texture,
                topRight, botRight,
                thickness, color);
        }

        public static void DrawRect(this SpriteBatch spriteBatch, Texture2D texture, Rectangle rect, int thickness, Color color)
        {
            DrawRect(
                spriteBatch, texture, 
                new Vector2(rect.Left, rect.Top), 
                new Vector2(rect.Right, rect.Bottom), 
                thickness, color);
        }

        public static void DrawRect(this SpriteBatch spriteBatch, Texture2D texture, NonAxisAlignedBoundingBox rect, int thickness, Color color)
        {
            spriteBatch.DrawLine(texture, rect.TopLeft, rect.TopRight, thickness, color);
            spriteBatch.DrawLine(texture, rect.TopLeft, rect.BotLeft, thickness, color);
            spriteBatch.DrawLine(texture, rect.TopRight, rect.BotRight, thickness, color);
            spriteBatch.DrawLine(texture, rect.BotLeft, rect.BotRight, thickness, color);
        }
    }
}
