using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Game.Entities;
using TimeSink.Engine.Core;
using Microsoft.Xna.Framework;

namespace Engine.Game.Entities
{
    public interface IInventoryItem
    {
        void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime);
    }
}
