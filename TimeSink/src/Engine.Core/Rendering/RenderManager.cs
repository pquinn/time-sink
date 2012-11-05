using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Engine.Core.Rendering
{
    public class RenderManager
    {
        HashSet<IRenderable> renderables = new HashSet<IRenderable>();

        public bool RegisterRenderable(IRenderable renderable)
        {
            return renderables.Add(renderable);
        }

        public bool UnregisterRenderable(IRenderable renderable)
        {
            return renderables.Remove(renderable);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var renderable in renderables)
            {
                renderable.Rendering.Draw(spriteBatch);
            }
        }
    }
}
