using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Rendering
{
    public enum RenderLayer
    {
        Background,
        Midground,
        Gameground,
        Foreground,
        UI //special case which will render fixed positions relative to the camera
    }
}
