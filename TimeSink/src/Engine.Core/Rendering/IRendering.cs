﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public interface IRendering
    {
        void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache);
    }
}
