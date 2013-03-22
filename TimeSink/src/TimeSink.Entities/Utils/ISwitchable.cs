using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Entities.Utils
{
    interface ISwitchable
    {
        bool Enabled { get; set; }
        void OnSwitch();
    }
}
