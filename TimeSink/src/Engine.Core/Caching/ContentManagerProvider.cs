using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TimeSink.Engine.Core.Caching
{
    public class ContentManagerProvider<T> : IResourceProvider<T>
    {
        private ContentManager contentManager;
        private string relativePath;

        public ContentManagerProvider(ContentManager contentManager)
        {
            this.contentManager = contentManager;
        }

        public T GetResource(string key)
        {
             return contentManager.Load<T>(key);
        }

        public IEnumerable<Tuple<string,T>> GetResources()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetResources(IEnumerable<string> keys)
        {
            var loadedResources = new List<T>();
            keys.ForEach(key => loadedResources.Add(contentManager.Load<T>(key)));

            return loadedResources;
        }
    }
}
