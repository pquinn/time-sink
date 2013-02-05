using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.StateManagement
{
    public class SoundOptionsScreen : MenuScreen
    {
        #region fields
        MenuEntry toggleSounds;
        MenuEntry toggleMusic;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SoundOptionsScreen()
            : base("Sounds")
        {
            // Create our menu entries.
            toggleSounds = new MenuEntry(string.Empty);
            toggleMusic = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            toggleSounds.Selected += ToggleSoundsSelected;
            toggleMusic.Selected += ToggleMusicSelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(toggleSounds);
            MenuEntries.Add(toggleMusic);
            MenuEntries.Add(back);
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            toggleSounds.Text = "Toggle Sounds Currently On?:" + EngineGame.Instance.SoundsEnabled.ToString(); // Show flag for on/off
            toggleMusic.Text = "Toggle Music Currently On?" + EngineGame.Instance.MusicEnabled.ToString();
        }

        #endregion

        public void ToggleSoundsSelected(object sender, PlayerIndexEventArgs e)
        {
            EngineGame.Instance.SoundsEnabled = !EngineGame.Instance.SoundsEnabled;

            SetMenuEntryText();

        }
        public void ToggleMusicSelected(object sender, PlayerIndexEventArgs e)
        {
            EngineGame.Instance.MusicEnabled = !EngineGame.Instance.MusicEnabled;

            SetMenuEntryText();
        }


    }
}
