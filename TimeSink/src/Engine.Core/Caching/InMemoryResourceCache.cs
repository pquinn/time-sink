using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Caching
{
    public class InMemoryResourceCache<T> : IResourceCache<T>
    {
        private IResourceProvider<T> provider;
        private Dictionary<string, T> cache;

        public InMemoryResourceCache(IResourceProvider<T> provider)
        {
            this.provider = provider;
            cache = new Dictionary<string, T>();
            FetchIfCacheMiss = true;
        }

        public virtual bool FetchIfCacheMiss { get; set; }

        public virtual T GetResource(string key)
        {
            T resource;
            if (!cache.TryGetValue(key, out resource))
            {
                if (FetchIfCacheMiss)
                {
                    resource = LoadResource(key);
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("The key, '{0}' was not present in the cache, and it is" +
                                      " not configured to fetch from provider if missed.", key));
                }
            }

            return resource;
        }

        public virtual IEnumerable<Tuple<string,T>> GetResources()
        {
            List<KeyValuePair<string, T>> resources = cache.ToList();
            if (resources.Count != 0)
            {
                return resources.Select(x => new Tuple<string, T>(x.Key, x.Value));
            }
            {
                if (FetchIfCacheMiss)
                {
                    return LoadResources();                    
                }
                else
                {
                    throw new InvalidOperationException(
                        "No resources were present in the cache, and it is" +
                        " not configured to fetch from the provider if missed.");
                }
            }
        }

        public virtual IEnumerable<T> GetResources(IEnumerable<string> keys)
        {
            var resources = new List<T>();
            var missed = new List<string>();
            T resource;
            foreach (var key in keys)
            {
                if (!cache.TryGetValue(key, out resource))
                {
                    missed.Add(key);
                }
                else
                {
                    resources.Add(resource);
                }
            }

            if (missed.Count > 0)
            {
                if (FetchIfCacheMiss)
                {
                    var items = provider.GetResources(missed).ToArray();
                    resources.AddRange(items);
                    for (int i = 0; i < missed.Count; i++)
                    {
                        cache.Add(missed[i], items[i]);
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format("The following keys, '{0}' could not be found in the cache " +
                            "and it is not configured to fetch from the provider after a miss.", string.Join(", ", missed)));
                }
            }

            return resources;
        }

        public virtual T LoadResource(string key)
        {
            var resource = provider.GetResource(key);
            
            if (cache.ContainsKey(key))
                cache.Remove(key);

            cache.Add(key, resource);

            return resource;
        }

        public virtual IEnumerable<Tuple<string, T>> LoadResources()
        {
            var resources = provider.GetResources().ToList();
            foreach (var resource in resources)
            {
                cache.Add(resource.Item1, resource.Item2);
            }

            return resources;
        }

        public virtual IEnumerable<T> LoadResources(IEnumerable<string> keys)
        {
            var keysArray = keys.ToArray();
            var resources = provider.GetResources(keys).ToList();
            for (int i = 0; i < keysArray.Length; i++)
            {
                cache.Add(keysArray[i], resources[i]);
            }

            return resources;
        }

        public virtual void AddResource(string key, T resource)
        {
            cache.Add(key, resource);
        }

        public virtual void AddResources(IEnumerable<Tuple<string, T>> resources)
        {
            resources.ForEach(resource => AddResource(resource.Item1, resource.Item2));
        }

        public virtual void Clear()
        {
            cache.Clear();
        }
    }
}
