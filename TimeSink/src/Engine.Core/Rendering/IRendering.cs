﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    public interface IRendering
    {
        void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache);

        void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Rectangle sourceRect);

        void DrawSelected(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Color color);

        bool Contains(Vector2 point, IResourceCache<Texture2D> cache);
    }
}
