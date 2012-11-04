using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameStateManagementSample
{
    public abstract class SlotItem
    {
        public Texture2D icon;

        public bool isPrimary;

        public bool isSecondary;

        public bool IsPrimary
        {
            get { return isPrimary; }
            set { isPrimary = value; }
        }

        public bool IsSecondary
        {
            get { return isSecondary; }
            set { isSecondary = value; }
        }

        public Texture2D Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        public abstract bool IsEmpty();

    }
}
