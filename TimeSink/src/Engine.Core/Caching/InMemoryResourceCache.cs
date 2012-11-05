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

        internal InMemoryResourceCache(IResourceProvider<T> provider)
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
                    resource = provider.GetResource(key);
                    cache.Add(key, resource);
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

        public virtual IEnumerable<T> GetResources()
        {
            List<T> resources = cache.Values.ToList();
            if (resources.Count == 0)
            {
                if (FetchIfCacheMiss)
                {
                    var missed = provider.GetResources().ToList();
                    foreach (var resource in missed)
                    {
                        cache.Add(resource.Item1, resource.Item2);
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "No resources were present in the cache, and it is" +
                        " not configured to fetch from the provider if missed.");
                }
            }

            return resources;
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
