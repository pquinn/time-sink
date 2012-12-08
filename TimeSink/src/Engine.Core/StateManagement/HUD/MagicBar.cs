using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.StateManagement;

namespace TimeSink.Engine.Core.StateManagement.HUD
{
    class MagicBar : HudBar
    { 
        #region Fields

        #endregion

        #region Properties

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
            outline.Width = (graphics.Viewport.Width / 45 / 2);
            outline.Height = (graphics.Viewport.Width / 10 / 2);

            spriteBatch.Draw(icon, outline, Color.White);
        }
    }
}
