using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Editor
{
    public class PreviewGame : XNAControl.XNAControlGame
    {
        SpriteBatch spriteBatch;

        private Texture2D texture;

        public PreviewGame(IntPtr windowHandle, int width, int height)
            :base(windowHandle, "Content", width, height)
        {
        }

        public void ChangeTexture(string textureToDisplay)
        {
            texture = Content.Load<Texture2D>(textureToDisplay);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
 
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            if (texture != null)
            {
                var origin = new Vector2(texture.Width / 2, texture.Height / 2);

                spriteBatch.Begin();

                spriteBatch.Draw(
                    texture,
                    new Vector2(
                        GraphicsDevice.Viewport.Width / 2,
                        GraphicsDevice.Viewport.Height / 2),
                    null,
                    Color.White,
                    0,
                    origin,
                    (origin.X < 64 && origin.Y < 64) ? 
                        new Vector2(2, 2) :
                        Vector2.One,
                    SpriteEffects.None,
                    0);

                spriteBatch.End();
            }
        }
    }
}
