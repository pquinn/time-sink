using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Rendering
{
    public struct BoundingBox
    {
        public BoundingBox(float min_x, float max_x, float min_y, float max_y)
            : this()
        {
            Min_X = min_x;
            Max_X = max_x;
            Min_Y = min_y;
            Max_Y = max_y;
        }

        public float Min_X { get; set; }

        public float Max_X { get; set; }

        public float Min_Y { get; set; }

        public float Max_Y { get; set; }
    }
}
