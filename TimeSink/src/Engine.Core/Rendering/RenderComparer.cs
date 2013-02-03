using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Rendering
{
    public class RenderComparer : IComparer<IRendering>
    {
        public int Compare(IRendering x, IRendering y)
        {
            if (x.DepthWithinLayer < y.DepthWithinLayer)
                return 1;
            else if (x.DepthWithinLayer > y.DepthWithinLayer)
                return -1;
            else
                return 0;
        }
    }
}
