using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorEnabledAttribute : Attribute
    {
        public EditorEnabledAttribute()
        {
        }
    }
}
