
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.StateManagement;

namespace TimeSink.Engine.Core.StateManagement.HUD
{
    class HealthBar : HudBar
    {
        Rectangle sourceRect;
        float maxHealth = 100;
        float currentHealth = 100;

        public HealthBar(Texture2D texture)
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
            outline.Width = (int)((float)((graphics.Viewport.Width / 45) * 14 ) * (float)(currentHealth / maxHealth));
            outline.Height = (graphics.Viewport.Width / 15);

            sourceRect.Width = 200;
            sourceRect.Height = outline.Height;
            sourceRect.Location = new Point(534, 0);

            spriteBatch.Draw(icon, outline, sourceRect, Color.White);
        }
        public void TakeDamage(float currentHealth, float maxHealth)
        {
            this.currentHealth = currentHealth;
        }
    }
}
