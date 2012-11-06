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
    }
}
