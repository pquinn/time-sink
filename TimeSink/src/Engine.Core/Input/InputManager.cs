using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TimeSink.Engine.Core.Input
{
    public class InputManager
    {
        private static object lockObject = new object();

        private static InputManager input;
        public static InputManager Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (input == null)
                        input = new InputManager();
                    return input;
                }
            }
            set { input = value; }
        }

        private InputManager() { }

        KeyboardState LastState = Keyboard.GetState();
        KeyboardState CurrentState;
        MouseState lastMouseState, currentMouseState;

        // Has player input new keys?
        public bool IsNewKey(Keys key)
        {
            CurrentState = Keyboard.GetState();
            bool result;
            result = CurrentState.IsKeyDown(key) &&
                     LastState.IsKeyUp(key);
            return result;
        }

        // Is a key pressed?
        public bool Pressed(Keys key)
        {
            if (LastState.IsKeyDown(key))
                return true;
            else
                return false;
        }

        public MouseState CurrentMouseState
        {
            get { return currentMouseState; }
        }

        public MouseState LastMoustState
        {
            get { return lastMouseState; }
        }

        // Update the keyboard state
        public void Update()
        {
            LastState = CurrentState;
            CurrentState = Keyboard.GetState();

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }
    }
}
