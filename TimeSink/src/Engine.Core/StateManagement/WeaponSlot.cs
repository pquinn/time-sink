using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink;
using Microsoft.Xna.Framework.Content;
using TimeSink.Engine.Core.StateManagement.HUD;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core.StateManagement
{
    public class WeaponSlot : IHudElement
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

        /**********************************
         * Item to be held in the slot
         **********************************/
        SlotItem item;

        #endregion

        #region Properties
        /************************************************************
        * Gets or sets the position at which to draw this menu entry.
        *************************************************************/
        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public Texture2D Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        public SlotItem Item
        {
            get { return item; }
            set { item = value; }
        }
        #endregion

        public WeaponSlot(SlotItem target, Texture2D blank)
        {
            this.item = target;
            this.icon = blank;
        }

        public virtual void Update(MenuScreen screen, GameTime gameTime)
        {
            // Not needed for now
        }


        public virtual void Draw(GameScreen screen, bool isSelected, GameTime gameTime)
        {
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            GraphicsDevice graphics = screenManager.GraphicsDevice;

            outline.Location = position;
            outline.Width = (graphics.Viewport.Width / 15);
            outline.Height = (graphics.Viewport.Width / 15);

            if (item.IsPrimary)
            {
                spriteBatch.Draw(icon, outline, Color.OrangeRed);
            }
            else if (item.IsSecondary)
            {
                spriteBatch.Draw(icon, outline, Color.Purple);
            }
            else
            {
                spriteBatch.Draw(icon, outline, Color.Gray);
            }

            spriteBatch.Draw(item.Icon, outline, Color.White);
        }

        public virtual bool IsSlot()
        {
            return true;
        }

        public virtual int GetHeight()
        {
            return outline.Height;
        }

        public virtual int GetWidth()
        {
            return outline.Width;
        }

        public virtual bool IsPrimary()
        {
            return Item.IsPrimary;
        }
        public virtual bool IsSecondary()
        {
            return Item.IsSecondary;
        }
        public virtual bool GameplayDraw()
        {
            if (IsPrimary() || IsSecondary())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
