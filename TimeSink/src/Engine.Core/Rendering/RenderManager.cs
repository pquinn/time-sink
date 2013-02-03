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

        public void Draw(SpriteBatch spriteBatch, Camera camera, bool forPreviews)
        {
            var comparer = new RenderComparer();
            var sets = new Dictionary<RenderLayer, List<IRendering>>();
            sets.Add(RenderLayer.Background, new List<IRendering>());
            sets.Add(RenderLayer.Midground, new List<IRendering>());
            sets.Add(RenderLayer.Gameground, new List<IRendering>());
            sets.Add(RenderLayer.Foreground, new List<IRendering>());
            sets.Add(RenderLayer.UI, new List<IRendering>());

            foreach (var renderable in renderables)
            {
                var rendering = forPreviews ? renderable.Preview : renderable.Rendering;
                sets[rendering.RenderLayer].Add(rendering);
            }

            foreach (var set in sets.Values)
            {
                set.Sort(comparer);
                foreach (var rendering in set)
                {
                    rendering.Draw(spriteBatch, TextureCache, camera.Transform);
                }
            }
        }

        public void Clear()
        {
            renderables.Clear();
        }
    }
}
