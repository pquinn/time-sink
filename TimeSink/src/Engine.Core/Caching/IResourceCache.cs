using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Caching
{
    public interface IResourceCache<T>
    {
        T GetResource(string key);
        IEnumerable<T> GetResources();
        IEnumerable<T> GetResources(IEnumerable<string> keys);

        void AddResource(string key, T resource);
        void AddResources(IEnumerable<Tuple<string, T>> resources);

        void Clear();
    }
}
