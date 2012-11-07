﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class StackableRendering : IRendering
    {
        Stack<IRendering> renderStack;
        Vector2 parentPosition;

        public StackableRendering(Stack<IRendering> renderingStack)
            : this(renderingStack, Vector2.Zero)
        { }

        public StackableRendering(Stack<IRendering> renderingStack, Vector2 parentPosition)
        {
            this.renderStack = renderingStack;
            this.parentPosition = parentPosition;
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Vector2 positionOffset)
        {
            foreach (var rendering in renderStack)
            {
                rendering.Draw(spriteBatch, cache, positionOffset + parentPosition);
            }
        }
    }
}
