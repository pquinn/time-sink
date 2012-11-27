using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Input;

namespace Editor.States
{
    public class CameraZoomState : DefaultEditorState
    {
        private bool leftClick;
        private bool rightClick;

        public CameraZoomState(Camera camera, IResourceCache<Texture2D> cache)
            : base(camera, cache)
        {
        }

        public override void Execute()
        {
            var leftMouse = InputManager.Instance.CurrentMouseState.LeftButton;
            var rightMouse = InputManager.Instance.CurrentMouseState.RightButton;
            var onScreen = MouseOnScreen();
            if (leftMouse == ButtonState.Pressed && !leftClick && onScreen)
            {
                leftClick = true;
                Camera.Scale += new Vector2(.1f, .1f);
            }
            else if (leftMouse == ButtonState.Released)
            {
                leftClick = false;
            }
            
            if (rightMouse == ButtonState.Pressed && !rightClick && onScreen)
            {
                rightClick = true;
                Camera.Scale -= new Vector2(.1f, .1f);
            }
            else if (rightMouse == ButtonState.Released)
            {
                rightClick = false;
            }
        }

        private Vector3 GetMousePosition()
        {
            return new Vector3(
                InputManager.Instance.CurrentMouseState.X,
                InputManager.Instance.CurrentMouseState.Y,
                0);
        }
    }
}
