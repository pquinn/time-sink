using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Editor.States
{
    public class CameraTranslateState : DefaultEditorState
    {
        private bool inDrag;
        private Vector3 lastMouse;
        private Vector3 negOffset;

        public override void Execute(Level level)
        {
            if (InputManager.Instance.IsNewKey(Keys.X))
            {
                Debugger.Break();
            }

            var leftMouse = InputManager.Instance.CurrentMouseState.LeftButton;
            if (leftMouse == ButtonState.Pressed && !inDrag)
            {
                inDrag = true;
                lastMouse = GetMousePosition();
            }
            else if (leftMouse == ButtonState.Pressed)
            {
                var mouse = GetMousePosition();
                negOffset = mouse - lastMouse;
                lastMouse = mouse;
            }
            else if (leftMouse == ButtonState.Released)
            {
                inDrag = false;
                negOffset = Vector3.Zero;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera, Level level)
        {
            camera.PanCamera(negOffset);
            base.Draw(spriteBatch, camera, level);
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
