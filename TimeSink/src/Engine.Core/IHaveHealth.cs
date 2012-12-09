﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core
{
    public interface IHaveHealth
    {
        float Health { get;  set; }
        HashSet<DamageOverTimeEffect> Dots { get; set; }
        void RegisterDot(DamageOverTimeEffect dot);
    }
}
