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

namespace SoundAPI
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SoundEffect soundEngine;
        SoundEffect logon;
        SoundEffect dtd;
        SoundEffectInstance soundEngineInstance;
        SoundEffect musicLoop;
        SoundObject obj;
        List<SoundObject> soundList = new List<SoundObject>();            
        KeyboardState oldState;
        bool isPlaying = false;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            oldState = Keyboard.GetState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            soundEngine = Content.Load<SoundEffect>("Dixie");
            logon = Content.Load<SoundEffect>("Windows Logon");
            dtd = Content.Load<SoundEffect>("DtD");
            musicLoop = Content.Load<SoundEffect>("ts");
            soundEngineInstance = musicLoop.CreateInstance();
            soundEngineInstance.IsLooped = true;


            obj = new SoundObject(soundEngine, Vector2.Zero);
            soundList.Add(obj);
            soundList.Add(new SoundObject(logon, Vector2.One));
            soundList.Add(new SoundObject(dtd, Vector2.Zero));

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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

        protected void UpdateKeyboard()
        {
            KeyboardState newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.Left))
            {
                if (isPlaying == true && obj.IsModular)
                {
                    obj.PanLeft();
                }
            }
            if (newState.IsKeyDown(Keys.Right))
            {
                if (isPlaying == true && obj.IsModular)
                {
                    obj.PanRight();
                }
            }
            if (newState.IsKeyDown(Keys.NumPad1))
            {
                if (!oldState.IsKeyDown(Keys.NumPad1))
                {
                    soundList[0].PlaySound(Vector2.Zero);
                }
            }
            if (newState.IsKeyDown(Keys.NumPad2))
            {
                if (!oldState.IsKeyDown(Keys.NumPad2))
                {
                    soundList[1].PlaySound(Vector2.Zero);
                }
            }
            if (newState.IsKeyDown(Keys.NumPad3))
            {
                if (!oldState.IsKeyDown(Keys.NumPad3))
                {
                    soundList[2].PlaySound(Vector2.Zero);
                }
            }
            if (newState.IsKeyDown(Keys.Space))
            {
                if (!oldState.IsKeyDown(Keys.Space))
                {
                    if (obj.Dynamic.State.Equals(SoundState.Stopped))
                    {
                        obj.PlaySound(Vector2.Zero);
                        isPlaying = true;
                    }
                    else
                    {
                        obj.TogglePauseSound();
                    }
                }
                else if (oldState.IsKeyDown(Keys.Space))
                {

                }
            }
            if (newState.IsKeyDown(Keys.L))
            {
                if (!oldState.IsKeyDown(Keys.Space))
                {
                    obj.Dynamic.IsLooped = !obj.Dynamic.IsLooped;
                }
                else if (oldState.IsKeyDown(Keys.Space))
                {

                }
            }
            oldState = newState;
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

            TimeSpan time = TimeSpan.Zero;

            time += gameTime.ElapsedGameTime;

            UpdateKeyboard();


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

            base.Draw(gameTime);
        }
    }
}
