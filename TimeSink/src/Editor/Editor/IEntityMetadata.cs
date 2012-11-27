using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor
{
    public interface IEntityMetadata
    {
        Guid Name { get; }
    }
}
