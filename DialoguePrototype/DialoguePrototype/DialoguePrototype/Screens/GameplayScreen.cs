#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

// Include the necessary SunBurn namespaces.
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Rendering.Forward;
#endregion

namespace DialoguePrototype
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Matrix view;
        Matrix projection;

        float pauseAlpha;

        InputAction pauseAction;
        InputAction startAction;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            startAction = new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { Keys.Enter },
                true);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("fonts/gamefont");

                //ScreenManager.TraceEnabled = true;

                // Load the content repository, which stores all assets imported via the editor.
                // This must be loaded before any other assets.
                ScreenManager.contentRepository = content.Load<ContentRepository>("Content");

                // Add objects and lights to the ObjectManager and LightManager. They accept
                // objects and lights in several forms:
                //
                //   -As scenes containing both dynamic (movable) and static objects and lights.
                //
                //   -As SceneObjects and lights, which can be dynamic or static, and
                //    (in the case of objects) are created from XNA Models or custom vertex / index buffers.
                //
                //   -As XNA Models, which can only be static.
                //

                // Load the scene and add it to the managers.
                Scene scene = content.Load<Scene>("Scenes/Scene");

                ScreenManager.sceneInterface.Submit(scene);
                
                // Load the scene environment settings.
                ScreenManager.environment = content.Load<SceneEnvironment>("Environment/Environment");

                // TODO: use this.Content to load your game content here

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Deactivate()
        {
            ScreenManager.sceneInterface.Unload();
            ScreenManager.sunBurnCoreSystem.Unload();

            ScreenManager.environment = null;

            base.Deactivate();
        }

        
        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
            
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
            }

            view = Matrix.CreateLookAt(Vector3.One * 50, Vector3.Zero, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f),
                    StarterGame.Instance.graphics.GraphicsDevice.Viewport.AspectRatio, 0.1f, ScreenManager.environment.VisibleDistance);

            // Update all contained managers.
            ScreenManager.sceneInterface.Update(gameTime);
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else if (startAction.Evaluate(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid("4cf17838-279c-11e2-b64d-109adda800ea")), null);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.End();

            // Render the scene.
            ScreenManager.sceneState.BeginFrameRendering(view, projection, gameTime, ScreenManager.environment, ScreenManager.frameBuffers, true);
            ScreenManager.sceneInterface.BeginFrameRendering(ScreenManager.sceneState);

            // Add custom rendering that should occur before the scene is rendered.

            ScreenManager.sceneInterface.RenderManager.Render();

            // Add custom rendering that should occur after the scene is rendered.

            ScreenManager.sceneInterface.EndFrameRendering();
            ScreenManager.sceneState.EndFrameRendering();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
