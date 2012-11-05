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

namespace TimeSink.Editor.Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : XNAControl.XNAControlGame
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<StaticMesh> staticMeshes;
        Texture2D groundTile;

        InMemoryResourceCache<Texture2D> textureCache;

        public Game1(IntPtr handle)
            :base(handle, "Content")
        {
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            staticMeshes = new List<StaticMesh>()
            {
                new StaticMesh(new Vector2(20, 20)),
                new StaticMesh(new Vector2(294, 20)),
                new StaticMesh(new Vector2(566, 20))
            };
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // setup caches            
            textureCache = new InMemoryResourceCache<Texture2D>(
                new ContentManagerProvider<Texture2D>(Content));

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textureCache.LoadResource("Textures/Ground_Tile1");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            foreach (var staticMesh in staticMeshes)
            {
                spriteBatch.Draw(textureCache.GetResource("Textures/Ground_Tile1"), staticMesh.Position, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
