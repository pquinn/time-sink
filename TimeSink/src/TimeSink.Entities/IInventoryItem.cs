using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities
{
    public interface IInventoryItem
    {
        void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime);
    }
}
