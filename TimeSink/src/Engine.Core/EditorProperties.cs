using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core
{
    public class EditorProperties
    {    
        public EditorProperties()
        {
        }

        public static EditorProperties Instance { get; set; }

        public bool ShowGridLines { get; set; }

        public int GridLineSpacing { get; set; }

        public bool EnableSnapping { get; set; }

        public int ResolutionX { get; set; }

        public int ResolutionY { get; set; }
    }
}
