using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;

namespace Editor.States
{
    public class CameraZoomState : DefaultEditorState
    {
        public CameraZoomState(Camera camera, IResourceCache<Texture2D> cache)
            : base(camera, cache)
        {
        }
    }
}
