using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.StateManagement
{
    public class ControllerOptionsScreen : MenuScreen
    {
        MenuEntry controllerEnable;


        /// <summary>
        /// Constructor.
        /// </summary>
        public ControllerOptionsScreen()
            : base("Sounds")
        {
            // Create our menu entries.
            controllerEnable = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            controllerEnable.Selected += ToggleControllerSelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(controllerEnable);
            MenuEntries.Add(back);
        }

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            controllerEnable.Text = "Using Gamepad :" + EngineGame.Instance.GamepadEnabled.ToString(); // Show flag for on/off
        }


        public void ToggleControllerSelected(object sender, PlayerIndexEventArgs e)
        {
            EngineGame.Instance.GamepadEnabled = !EngineGame.Instance.GamepadEnabled;

            SetMenuEntryText();

        }
    }
}
