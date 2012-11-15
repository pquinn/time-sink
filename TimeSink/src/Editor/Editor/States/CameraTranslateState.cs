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
using TimeSink.Engine.Core.Caching;

namespace Editor.States
{
    public class CameraTranslateState : DefaultEditorState
    {
        private bool inDrag;
        private Vector3 dragPivot;
        private Vector3 cameraStart;
        private Vector3 negOffset;

        public CameraTranslateState(Camera camera, IResourceCache<Texture2D> cache)
            : base(camera, cache)
        {
        }

        public override void Execute()
        {
            if (InputManager.Instance.IsNewKey(Keys.X))
            {
                Debugger.Break();
            }

            var leftMouse = InputManager.Instance.CurrentMouseState.LeftButton;
            if (leftMouse == ButtonState.Pressed && !inDrag)
            {
                inDrag = true;
                dragPivot = GetMousePosition();
                cameraStart = Camera.Position;
            }
            else if (leftMouse == ButtonState.Pressed)
            {
                var mouse = GetMousePosition();
                Camera.Position = mouse - dragPivot + cameraStart;
            }
            else if (leftMouse == ButtonState.Released)
            {
                inDrag = false;
                negOffset = Vector3.Zero;
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
