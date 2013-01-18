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

        public CameraZoomState(Game game, Camera camera, IResourceCache<Texture2D> cache)
            : base(game, camera, cache)
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
                var halfScreen = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
                var mousePos = new Vector3(GetMousePosition(), 0);
                Camera.ZoomCamera(new Vector2(1.5f, 1.5f), mousePos);
                Camera.TranslateCamera(new Vector3(halfScreen, 0) - mousePos);
            }
            else if (leftMouse == ButtonState.Released)
            {
                leftClick = false;
            }
            
            if (rightMouse == ButtonState.Pressed && !rightClick && onScreen)
            {
                rightClick = true;
                var halfScreen = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
                var mousePos = new Vector3(GetMousePosition(), 0);
                Camera.ZoomCamera(new Vector2(.667f, .667f), mousePos);
                Camera.TranslateCamera(new Vector3(halfScreen, 0) - mousePos);
            }
            else if (rightMouse == ButtonState.Released)
            {
                rightClick = false;
            }
        }
    }
}
