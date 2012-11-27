using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.StateManagement;

namespace TimeSink.Engine.Core.StateManagement.HUD
{
    public interface IHudElement
    {
        //Update the element every game tick
         void Update(MenuScreen menu, GameTime gameTime);

        //Draw the component on the screen
         void Draw(GameScreen screen, bool isSelected, GameTime gameTime);

        //Is this a game slot?
         bool IsSlot();

        //Draw this always during gameplay
         bool GameplayDraw();

         int GetWidth();

         Point Position
         {
             get;
             set;
         }
    }
}
