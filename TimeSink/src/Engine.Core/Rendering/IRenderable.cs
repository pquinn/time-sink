﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Rendering
{
    public interface IRenderable
    {
        List<IRendering> Renderings { get; }

        string EditorName { get; }

        IRendering Preview { get; }
    }
}
