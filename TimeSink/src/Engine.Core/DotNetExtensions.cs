using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core
{
    public static class DotNetExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static void ReplaceOrAdd(this Dictionary<string, object> dict, string key, object value)
        {
            object obj = null;
            var has = dict.TryGetValue(key, out obj);

            if (has)
                dict[key] = value;
            else
                dict.Add(key, value);
        }
    }
}
