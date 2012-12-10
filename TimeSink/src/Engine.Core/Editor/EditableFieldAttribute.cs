using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Editor
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited=true)]
    public class EditableFieldAttribute : Attribute
    {
        public EditableFieldAttribute(string display)
        {
            Display = display;
        }

        public string Display { get; private set; }
    }
}
