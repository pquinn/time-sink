using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;

namespace GameStateManagementSample
{
    class WeaponSlot
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
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
        }


        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            GraphicsDevice graphics = screenManager.GraphicsDevice;

            outline.Location = position;
            outline.Width = (graphics.PresentationParameters.BackBufferWidth / 20);
            outline.Height = (graphics.PresentationParameters.BackBufferWidth / 20);

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

        public virtual int GetHeight()
        {
            return outline.Height;
        }

        public virtual int GetWidth()
        {
            return outline.Width;
        }
    }
}
