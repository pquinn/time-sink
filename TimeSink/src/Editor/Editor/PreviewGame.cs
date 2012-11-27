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
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;

namespace Editor
{
    public class PreviewGame : XNAControl.XNAControlGame
    {
        SpriteBatch spriteBatch;

        IResourceCache<Texture2D> textureCache;

        private Entity entity;

        public PreviewGame(IntPtr windowHandle, int width, int height)
            :base(windowHandle, "Content", width, height)
        {
        }

        public void ChangePreview(Entity entity)
        {
            this.entity = entity;
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

            textureCache = new InMemoryResourceCache<Texture2D>(
                new ContentManagerProvider<Texture2D>(Content));

            spriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            if (entity != null)
            {
                var camera = Matrix.CreateTranslation(
                        GraphicsDevice.Viewport.Width / 2,
                        GraphicsDevice.Viewport.Height / 2,
                        0);

                entity.Preview.Draw(spriteBatch, textureCache, camera);
            }
        }
    }
}
