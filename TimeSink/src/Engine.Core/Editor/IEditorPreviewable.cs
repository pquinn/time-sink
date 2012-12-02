using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core.Editor
{
    public interface IEditorPreviewable
    {
        string EditorName  { get; }
        IRendering Rendering { get; }
    }
}
