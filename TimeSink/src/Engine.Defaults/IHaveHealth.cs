using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Defaults
{
    public interface IHaveHealth
    {
        float Health { get;  set; }
        HashSet<DamageOverTimeEffect> Dots { get; set; }
        void RegisterDot(DamageOverTimeEffect dot);
    }
}
