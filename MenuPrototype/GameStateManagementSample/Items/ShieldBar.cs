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
    class ShieldBar : HudBar
    {
       public ShieldBar(Texture2D texture)
       {
           this.icon = texture;
       }

        public override void Update(MenuScreen menu, Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        public override void Draw(GameScreen screen, bool isSelected, Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            GraphicsDevice graphics = screenManager.GraphicsDevice;


            outline.Location = position;
            outline.Width = ((graphics.PresentationParameters.BackBufferWidth / 45) * 14);
            outline.Height = (graphics.Viewport.Width / 30);

            spriteBatch.Draw(icon, outline, Color.SkyBlue);
        }
    }
}
