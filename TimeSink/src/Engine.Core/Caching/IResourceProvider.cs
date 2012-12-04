using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Caching
{
    public interface IResourceProvider<T>
    {
        T GetResource(string key);
        IEnumerable<Tuple<string, T>> GetResources();
        IEnumerable<T> GetResources(IEnumerable<string> keys);
    }
}
