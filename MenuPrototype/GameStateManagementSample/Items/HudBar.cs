using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameStateManagementSample.Items
{
   abstract class HudBar : IHudElement
   {
       /**************************************
      * The image used to represent the weapon
      **************************************/
       public Texture2D icon;

       /******************************************
        * The outline of the weapon slot
        ******************************************/
       public Rectangle outline;

       /**********************************
        * The position of the slot
        *********************************/
      public  Point position;

       public abstract void Update(MenuScreen menu, Microsoft.Xna.Framework.GameTime gameTime);


        public abstract void Draw(GameStateManagement.GameScreen screen, bool isSelected, Microsoft.Xna.Framework.GameTime gameTime);


        public bool IsSlot()
        {
            return false;
        }

        public bool GameplayDraw()
        {
            return true;
        }

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public  int GetWidth()
        {
            return outline.Width;
        }

    }
}
