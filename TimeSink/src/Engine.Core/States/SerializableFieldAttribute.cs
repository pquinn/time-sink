using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.States
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, Inherited=true)]
    public class SerializableFieldAttribute : Attribute
    {
        public SerializableFieldAttribute()
        {
        }
    }
}
