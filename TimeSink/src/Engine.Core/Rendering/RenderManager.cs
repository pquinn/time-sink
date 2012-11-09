using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    public class RenderManager
    {
        HashSet<IRenderable> renderables = new HashSet<IRenderable>();

        public RenderManager(IResourceCache<Texture2D> cache)
        {
            TextureCache = cache;
        }

        public IResourceCache<Texture2D> TextureCache { get; set; }

        public bool RegisterRenderable(IRenderable renderable)
        {
            return renderables.Add(renderable);
        }

        public bool RegisterRenderable(IEnumerable<IRenderable> renderablesToRegister)
        {
            bool hasFailure = false;
            renderablesToRegister.ForEach(x => hasFailure &= renderables.Add(x));

            return hasFailure;
        }

        public bool UnregisterRenderable(IRenderable renderable)
        {
            return renderables.Remove(renderable);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var renderable in renderables)
            {
                renderable.Rendering.Draw(
                    spriteBatch, 
                    TextureCache, 
                    Vector2.Zero, 
                    0.0f,
                    Vector2.One
                );
            }

            spriteBatch.End();
        }
    }
}
