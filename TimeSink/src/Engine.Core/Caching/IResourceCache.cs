using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Caching
{
    public interface IResourceCache<T>
    {
        T GetResource(string key);
        IEnumerable<Tuple<string, T>> GetResources();
        IEnumerable<T> GetResources(IEnumerable<string> keys);

        void AddResource(string key, T resource);
        void AddResources(IEnumerable<Tuple<string, T>> resources);
        
        T LoadResource(string key);
        IEnumerable<Tuple<string, T>> LoadResources();
        IEnumerable<T> LoadResources(IEnumerable<string> keys);

        void Clear();
    }
}
