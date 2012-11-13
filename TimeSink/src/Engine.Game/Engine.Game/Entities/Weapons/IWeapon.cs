using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Game.Entities;
using TimeSink.Engine.Core;
using Microsoft.Xna.Framework;
using Engine.Game.Entities;

namespace TimeSink.Engine.Game
{
    public interface IWeapon : IInventoryItem
    {
        void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime);
    }
}
