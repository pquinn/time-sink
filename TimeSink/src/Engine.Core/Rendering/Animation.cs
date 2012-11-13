using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Rendering
{
    public class Animation
    {
        private int totalFrames;
        private int currentFrame = 0;
        private BasicRendering rendering;

        public int TotalFrames
        {
            get { return totalFrames; }
            set { totalFrames = value; }
        }

        public BasicRendering Rendering
        {
            get { return rendering; }
            set { rendering = value; }
        }

        public Animation(int totalFrames, string texture, int maxWidth, int maxHeight, Vector2 loc)
        {
            this.totalFrames = totalFrames;
            this.rendering = new BasicRendering(texture, loc, 0, Vector2.Zero, new Rectangle(0, 0, maxWidth, maxHeight));
        }

        public void UpdateSourceRect()
        {
            if (rendering.SrcRectangle.HasValue)
            {
                Rectangle rect = ((Rectangle)rendering.SrcRectangle);
                rect.X = currentFrame * rect.Width;
                rect.Y = 0;
            }
        }
        public void UpdateFrame()
        {
            if (currentFrame >= totalFrames)
            {
                currentFrame = 0;
            }
            else
                currentFrame++;
        }
    }
}
