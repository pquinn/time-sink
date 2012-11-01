using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MechanicsTest.Controller
{
    interface IKeyboardControllable
    {
        void HandleKeyboardInput(GameTime gameTime);
    }
}
