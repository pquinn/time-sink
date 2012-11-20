using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameStateManagementSample.Items
{
    class HealthBar : HudBar
    {
        Rectangle sourceRect;
        public HealthBar(Texture2D texture)
        {
            this.icon = texture;
        }
        public override void Update(MenuScreen menu, Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        public override void Draw(GameStateManagement.GameScreen screen, bool isSelected, Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            GraphicsDevice graphics = screenManager.GraphicsDevice;


            outline.Location = position;
            outline.Width = (int)((graphics.Viewport.Width / 45) * 14 );
            outline.Height = (graphics.Viewport.Width / 15);

            sourceRect.Width = 200;
            sourceRect.Height = outline.Height;
            sourceRect.Location = new Point(534, 0);

            spriteBatch.Draw(icon, outline, sourceRect, Color.White);
        }
    }
}
