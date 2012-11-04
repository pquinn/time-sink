using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;

namespace GameStateManagementSample
{
    class ShieldBar : IHudElement
    {
        #region Fields
        /**************************************
         * The image used to represent the weapon
         **************************************/
        Texture2D icon;

        /******************************************
         * The outline of the weapon slot
         ******************************************/
        Rectangle outline;

        /**********************************
         * The position of the slot
         *********************************/
        Point position;

        #endregion

        public void Update(MenuScreen menu, Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Draw(GameScreen screen, bool isSelected, Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsSlot()
        {
            return false;
        }
    }
}
