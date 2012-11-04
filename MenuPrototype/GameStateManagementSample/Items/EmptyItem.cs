using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameStateManagementSample.Items
{
    class EmptyItem : SlotItem
    {
        public EmptyItem()
        { }

        public override bool IsEmpty()
        {
            return true;
        }
    }
}
