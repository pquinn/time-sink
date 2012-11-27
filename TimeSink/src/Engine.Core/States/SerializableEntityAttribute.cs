using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TimeSink.Engine.Core.States
{
    public class SerializableEntityAttribute : Attribute
    {
        public SerializableEntityAttribute(string guid)
        {
            Id = new Guid(guid);
        }

        public Guid Id { get; set; }
    }
}
