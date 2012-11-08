using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using GameStateManagementSample.Items;

namespace GameStateManagementSample
{
    class MagicBar : HudBar
    { 
        #region Fields

        #endregion

        #region Properties
        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public MagicBar(Texture2D texture)
        {
            this.icon = texture;
        }
        #endregion
        public override void Update(MenuScreen menu, GameTime gameTime)
        {
        }

        public override void Draw(GameScreen screen, bool isSelected, GameTime gameTime)
        {
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            GraphicsDevice graphics = screenManager.GraphicsDevice;


            outline.Location = position;
            outline.Width = (graphics.PresentationParameters.BackBufferWidth / 45);
            outline.Height = (graphics.PresentationParameters.BackBufferWidth / 10);

            spriteBatch.Draw(icon, outline, Color.White);

        }

        public virtual int GetWidth()
        {
            return outline.Width;
        }
    }
}
