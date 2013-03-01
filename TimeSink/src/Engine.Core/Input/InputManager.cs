using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Input
{
    public class InputManager
    {
        private static object lockObject = new object();

        private Dictionary<ButtonActions, string> keyboardTextures;
        private Dictionary<ButtonActions, string> gamepadTextures;


        public enum ButtonActions
        {
            MoveLeft, MoveRight, Jump, Sprint, Shoot, UpAction, DownAction, AimUp, AimLeft, AimRight, AimDown, Interact, Pickup
        }

        private Dictionary<ButtonActions, Keys> keyDictionary;
        private Dictionary<ButtonActions, Buttons> gamepadDictionary;
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

        private InputManager() { InitializeDict(); }

        KeyboardState LastKeyState = Keyboard.GetState();
        GamePadState LastPadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
        KeyboardState CurrentKeyState;
        GamePadState CurrentPadState;
        MouseState lastMouseState, currentMouseState;

        public Dictionary<ButtonActions, string> GamepadTextures
        {
            get { return gamepadTextures; }
            set { }
        }
        
        public Dictionary<ButtonActions, string> KeyboardTextures
        {
            get { return keyboardTextures; }
            set { }
        }

        public void InitializeDict()
        {
            keyDictionary = new Dictionary<ButtonActions, Keys>();
            gamepadDictionary = new Dictionary<ButtonActions, Buttons>();
            keyboardTextures = new Dictionary<ButtonActions, string>();
            gamepadTextures = new Dictionary<ButtonActions, string>();

            keyDictionary.Add(ButtonActions.MoveLeft, Keys.A);
            keyDictionary.Add( ButtonActions.MoveRight, Keys.D);
            keyDictionary.Add( ButtonActions.DownAction,Keys.S);
            keyDictionary.Add(ButtonActions.UpAction,Keys.W );
            keyDictionary.Add(ButtonActions.Shoot, Keys.F );
            keyDictionary.Add( ButtonActions.Interact,Keys.E);
            keyDictionary.Add(ButtonActions.Pickup, Keys.X);
            keyDictionary.Add( ButtonActions.Sprint,Keys.LeftShift);
            keyDictionary.Add( ButtonActions.AimUp,Keys.Up);
            keyDictionary.Add( ButtonActions.AimDown,Keys.Down);
            keyDictionary.Add(ButtonActions.AimLeft,Keys.Left );
            keyDictionary.Add( ButtonActions.AimRight, Keys.Right);
            keyDictionary.Add(ButtonActions.Jump, Keys.Space);

            gamepadDictionary.Add(ButtonActions.MoveLeft, Buttons.DPadLeft);
            gamepadDictionary.Add(ButtonActions.MoveRight, Buttons.DPadRight);
            gamepadDictionary.Add(ButtonActions.DownAction, Buttons.DPadDown);
            gamepadDictionary.Add(ButtonActions.UpAction, Buttons.DPadUp);
            gamepadDictionary.Add(ButtonActions.Shoot, Buttons.RightTrigger);
            gamepadDictionary.Add(ButtonActions.Interact, Buttons.Y);
            gamepadDictionary.Add(ButtonActions.Sprint, Buttons.X);
            gamepadDictionary.Add(ButtonActions.AimUp, Buttons.RightShoulder);
            gamepadDictionary.Add(ButtonActions.AimDown, Buttons.LeftShoulder);
            gamepadDictionary.Add(ButtonActions.Jump, Buttons.A);
            gamepadDictionary.Add(ButtonActions.Pickup, Buttons.B);

            keyboardTextures.Add(ButtonActions.MoveLeft, "Textures/Keys/a-Key");
            keyboardTextures.Add(ButtonActions.MoveRight, "Textures/Keys/d-Key");
            keyboardTextures.Add(ButtonActions.DownAction, "Textures/Keys/s-Key");
            keyboardTextures.Add(ButtonActions.UpAction, "Textures/Keys/w-Key");
            keyboardTextures.Add(ButtonActions.Shoot, "Textures/Keys/f-Key");
            keyboardTextures.Add(ButtonActions.Interact, "Textures/Keys/e-Key");
            keyboardTextures.Add(ButtonActions.Sprint, "Textures/Keys/shift-Key");
            keyboardTextures.Add(ButtonActions.AimUp, "Textures/Keys/upArrow");
            keyboardTextures.Add(ButtonActions.AimDown, "Textures/Keys/downArrow");
            keyboardTextures.Add(ButtonActions.Jump, "Textures/Keys/space-Key");
            keyboardTextures.Add(ButtonActions.Pickup, "Textures/Keys/x-Key");

            gamepadTextures.Add(ButtonActions.MoveLeft, "Textures/Keys/PS/");
            gamepadTextures.Add(ButtonActions.MoveRight, "Textures/Keys/PS/");
            gamepadTextures.Add(ButtonActions.DownAction, "Textures/Keys/PS/");
            gamepadTextures.Add(ButtonActions.UpAction, "Textures/Keys/PS/");
            gamepadTextures.Add(ButtonActions.Shoot, "Textures/Keys/PS/");
            gamepadTextures.Add(ButtonActions.Interact, "Textures/Keys/PS/PS-TRI");
            gamepadTextures.Add(ButtonActions.Sprint, "Textures/Keys/PS/PS-SQ");
            gamepadTextures.Add(ButtonActions.AimUp, "Textures/Keys/PS/");
            gamepadTextures.Add(ButtonActions.AimDown, "Textures/Keys/PS/");
            gamepadTextures.Add(ButtonActions.Jump, "Textures/Keys/PS/PS-X");
            gamepadTextures.Add(ButtonActions.Pickup, "Textures/Keys/PS/PS-O");
           // gamepadDictionary.Add(ButtonActions.AimLeft, Keys.Left);
           // gamepadDictionary.Add(ButtonActions.AimRight, Keys.Right); sticks

        }

        // Has player input new keys?
        public bool IsNewKey(Keys key)
        {
            CurrentKeyState = Keyboard.GetState();
            bool result;
            result = CurrentKeyState.IsKeyDown(key) &&
                     LastKeyState.IsKeyUp(key);
            return result;
        }

        // Is a key pressed?
        public bool Pressed(Keys key)
        {
            if (LastKeyState.IsKeyDown(key))
            {
                return true;
            }
            else
                return false;
        }

        public bool Pressed(Buttons button)
        {

            if (LastPadState.IsButtonDown(button))
            {
                Console.WriteLine(LastPadState.DPad.Right.ToString());
                return true;
            }
            else
                return false;
        }

        public bool IsNewKey(Buttons button)
        {
            CurrentPadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            bool result;
            result = CurrentPadState.IsButtonDown(button) &&
                     LastPadState.IsButtonUp(button);
            return result;
        }

        public bool ActionHeld(ButtonActions b)
        {
            if (Pressed(keyDictionary[b]) || Pressed(gamepadDictionary[b]))
            {
                return true;
            }
            else
                return false;
        }
        public bool ActionPressed(ButtonActions b)
        {
            if (IsNewKey(keyDictionary[b]) || IsNewKey(gamepadDictionary[b]))
            {
                return true;
            }
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
            LastKeyState = CurrentKeyState;
            CurrentKeyState = Keyboard.GetState();

            LastPadState = CurrentPadState;
            CurrentPadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);


            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        internal static void ForceMousePosition(Vector2 point)
        {
            Mouse.SetPosition((int) point.X, (int) point.Y);
        }

        public bool NoButtonsPressed(GamePadState gp)
        {
            List<Buttons> buttonList = new List<Buttons>()
            {
                Buttons.A, Buttons.B, Buttons.Back, Buttons.BigButton,
                Buttons.DPadDown, Buttons.DPadRight, Buttons.DPadLeft, Buttons.DPadUp,
                Buttons.LeftShoulder, Buttons.LeftStick, Buttons.LeftTrigger,
                Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp, 
                Buttons.RightShoulder, Buttons.RightStick, Buttons.RightTrigger,
                Buttons.RightThumbstickDown, Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight, Buttons.RightThumbstickUp,
                Buttons.Start, Buttons.X, Buttons.Y
            };

            foreach(Buttons b in buttonList)
            {
                if (gp.IsButtonDown(b))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
