using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.States
{
    [Serializable]
    public struct Pair<K, V>
    {
        public K Key
        { get; set; }

        public V Value
        { get; set; }
    }
}
