using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities
{
    public interface IInventoryItem : IMenuItem
    {
        void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged);
        void ChargeInitiated(UserControlledCharacter character, GameTime gameTime);
        void ChargeReleased(UserControlledCharacter character, GameTime gameTime);
    }
}
