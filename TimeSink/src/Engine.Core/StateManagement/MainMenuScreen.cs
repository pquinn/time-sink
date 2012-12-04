#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Microsoft.Win32;
#endregion

namespace TimeSink.Engine.Core.StateManagement
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
   public class MainMenuScreen : MenuScreen
    {
        #region Initialization


        ContentManager content;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Time Sink")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry loadMenuEntry = new MenuEntry("Load");
            MenuEntry holder2 = new MenuEntry("Map");
            MenuEntry holder3 = new MenuEntry("Quests");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

         
            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;
            loadMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(loadMenuEntry_Selected);

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(loadMenuEntry);
            MenuEntries.Add(holder2);
            MenuEntries.Add(holder3);
            MenuEntries.Add(exitMenuEntry);


        }

        public MenuEntry PlayGameMenuEntry { get; set; }
        public MenuEntry OptionsMenuEntry { get; set; }
        public MenuEntry LoadMenuEntry { get; set; }
        public MenuEntry ExitMenuEntry { get; set; }

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
                content = ScreenManager.Game.Content;
            }
        }
       
        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameplayScreen gp = new GameplayScreen(EngineGame.Instance.LevelManager);   
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               gp);
            ScreenManager.CurrentGameplay = gp;
        }
       
        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        void loadMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            // Configure open file dialog box
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document  
                ScreenManager.GameWorld.LevelManager.Clear();
                ScreenManager.GameWorld.LevelManager.DeserializeLevel(dlg.FileName);
            }
        }
        #endregion
    }
}
