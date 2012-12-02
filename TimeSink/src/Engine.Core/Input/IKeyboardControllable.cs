using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Input
{
    interface IKeyboardControllable
    {
        void HandleKeyboardInput(GameTime gameTime, EngineGame world);
    }
}
