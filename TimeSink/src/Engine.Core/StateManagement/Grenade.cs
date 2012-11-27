using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.StateManagement.HUD
{
    class Grenade : SlotItem
    {
        public Grenade(Texture2D texture)
        {
            this.icon = texture;
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
