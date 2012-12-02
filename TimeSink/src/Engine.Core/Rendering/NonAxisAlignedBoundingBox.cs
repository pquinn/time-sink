using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    public struct NonAxisAlignedBoundingBox
    {
        public NonAxisAlignedBoundingBox(Vector2 topLeft, Vector2 topRight, Vector2 botLeft, Vector2 botRight)
            : this()
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BotLeft = botLeft;
            BotRight = botRight;
        }

        public Vector2 TopLeft { get; set; }

        public Vector2 TopRight { get; set; }

        public Vector2 BotLeft { get; set; }

        public Vector2 BotRight { get; set; }
    }
}
