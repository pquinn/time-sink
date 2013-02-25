using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Input;

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

        private float getDepthScale(RenderLayer l)
        {
            switch (l)
            {
                case RenderLayer.Background:
                    return .6f;
                default:
                    return 1;
            }
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
                if (forPreviews)
                    sets[renderable.Preview.RenderLayer].Add(renderable.Preview);
                else
                {
                    renderable.Renderings.ForEach( 
                        x => sets[x.RenderLayer].Add(x));
                }
            }

            if (InputManager.Instance.Pressed(Microsoft.Xna.Framework.Input.Keys.B))
            {
            }

            foreach (var layerSetPair in sets)
            {
                var set = layerSetPair.Value;
                set.Sort(comparer);
                foreach (var rendering in set)
                {
                    if (layerSetPair.Key == RenderLayer.UI)
                    {
                        rendering.Draw(spriteBatch, TextureCache, Matrix.Identity);
                    }
                    else
                    {
                        var t = camera.Transform;
                        t.Translation *= getDepthScale(layerSetPair.Key);
                        rendering.Draw(spriteBatch, TextureCache, t);
                    }
                }
            }
        }

        public void Clear()
        {
            renderables.Clear();
        }
    }
}
