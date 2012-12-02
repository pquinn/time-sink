using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Engine.Core.Editor
{
    public class EditorRenderManager
    {
        HashSet<IEditorPreviewable> previewables = new HashSet<IEditorPreviewable>();

        public EditorRenderManager(IResourceCache<Texture2D> cache)
        {
            TextureCache = cache;
        }

        public IResourceCache<Texture2D> TextureCache { get; set; }

        public bool RegisterPreviewable(IEditorPreviewable previewable)
        {
            return previewables.Add(previewable);
        }

        public bool RegisterPreviewable(IEnumerable<IEditorPreviewable> previewablesToRegister)
        {
            bool hasFailure = false;
            previewablesToRegister.ForEach(x => hasFailure &= previewables.Add(x));

            return hasFailure;
        }

        public bool UnregisterPreviewable(IEditorPreviewable previewable)
        {
            return previewables.Remove(previewable);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (var previewable in previewables)
            {
                previewable.Rendering.Draw(
                    spriteBatch,
                    TextureCache,
                    camera.Transform
                );
            }
        }

        public void Clear()
        {
            previewables.Clear();
        }
    }
}
