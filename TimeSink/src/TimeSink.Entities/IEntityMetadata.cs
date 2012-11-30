using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Entities
{
    public interface IEntityMetadata
    {
        Guid Name { get; }
    }
}
