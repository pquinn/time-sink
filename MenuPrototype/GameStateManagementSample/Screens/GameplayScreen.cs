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
using System.Collections.Generic;
using GameStateManagementSample.Items;
using DialoguePrototype;
#endregion

namespace GameStateManagementSample
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
        
        Texture2D blank;
        Texture2D grenade;
        Texture2D empty;
        Texture2D outline;
        Texture2D shield;
        Texture2D trans;

        public const int MAX_WEAPON_SLOTS = 9;

        public int currentSlots = 0;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);


        List<IHudElement> hudElements = new List<IHudElement>();
        List<Rectangle> transparencies = new List<Rectangle>();

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;
        InputAction startAction;

        public List<IHudElement> HudElements
        {
            get { return hudElements; }
            set { hudElements = value; }
        }

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
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("gamefont");


                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
                content = ScreenManager.Game.Content;
                blank = content.Load<Texture2D>("blank");
                grenade = content.Load<Texture2D>("grenade1");
                empty = content.Load<Texture2D>("Empty");
                outline = content.Load<Texture2D>("Slot Outline-01");
                shield = content.Load<Texture2D>("shield");
                trans = content.Load<Texture2D>("hBar");

                CreateMenuItems();
            }
        }

        public void AddWeaponSlot(SlotItem item)
        {
            WeaponSlot slot = new WeaponSlot(item, outline);

            HudElements.Add(slot);

            currentSlots++;
        }

        public void CreateMenuItems()
        {
            SlotItem grenadeItem = new Grenade(grenade);
            SlotItem grenadeItemBackup = new Grenade(grenade);
            SlotItem grenadeItemBackup2 = new Grenade(grenade);
            SlotItem blankItem = new Grenade(empty);
            MagicBar mBar = new MagicBar(shield);
            HealthBar hBar = new HealthBar(trans);
            ShieldBar sBar = new ShieldBar(empty);
            Rectangle hBarTrans = new Rectangle(0, 0, 200, 75);

            grenadeItem.IsPrimary = true;
            grenadeItemBackup.IsSecondary = true;

            transparencies.Add(hBarTrans);
            hudElements.Add(mBar);
            hudElements.Add(hBar);
            hudElements.Add(sBar);
            AddWeaponSlot(grenadeItem);
            AddWeaponSlot(grenadeItemBackup);
            AddWeaponSlot(grenadeItemBackup2);
            for (int i = currentSlots; i <= MAX_WEAPON_SLOTS; i++)
            {
                AddWeaponSlot(grenadeItemBackup2);
            }
        }




        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
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
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
          
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {

                UpdateHudElements();
            }
        }

        public void UpdateHudElements()
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Point posn = new Point(0, 0);
                for (int i = 0; i < hudElements.Count; i++)
                {
                    IHudElement hudElement = HudElements[i];

                    if (hudElement.GetType().IsAssignableFrom(new HealthBar(null).GetType()))
                    {
                      posn.Y += ScreenManager.GraphicsDevice.Viewport.Width / 30;
                    }
                    
                    if (ScreenState == ScreenState.TransitionOn)
                    {
                        posn.Y -= (int)(transitionOffset * 50);
                    }

                    else if (!hudElement.GameplayDraw())
                    {
                        posn.Y -= (int)(transitionOffset * 512);
                    }
                    hudElement.Position = posn;

                    if (!hudElement.GetType().IsAssignableFrom(new HealthBar(null).GetType()))
                        posn.X += hudElement.GetWidth();

                    posn.Y = 0;

                }
            
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
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(hudElements), ControllingPlayer);
#endif
            }
            else if (startAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid("4cf17838-279c-11e2-b64d-109adda800ea")), null);
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (input.TouchState.Count > 0)
                {
                    Vector2 touchPosition = input.TouchState[0].Position;
                    Vector2 direction = touchPosition - playerPosition;
                    direction.Normalize();
                    movement += direction;
                }

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 8f;
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

            spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);

            spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
                                   enemyPosition, Color.DarkRed);

            // Draw each weaponSlot in turn.
            for (int i = 0; i < hudElements.Count; i++)
            {
                IHudElement hudElement = hudElements[i];
                 
               // bool isSelected = IsActive && (i == selectedEntry);
                if (hudElement.GameplayDraw())
                {
                    hudElement.Draw(this, false, gameTime);
                }
            }

            spriteBatch.End();

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
