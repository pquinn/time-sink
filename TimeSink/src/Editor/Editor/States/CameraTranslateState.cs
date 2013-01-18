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

        public CameraTranslateState(Game game, Camera camera, IResourceCache<Texture2D> cache)
            : base(game, camera, cache)
        {
        }

        public override void Execute()
        {
            var leftMouse = InputManager.Instance.CurrentMouseState.LeftButton;
            var onScreen = MouseOnScreen();
            if (leftMouse == ButtonState.Pressed && !inDrag && onScreen)
            {
                inDrag = true;
                dragPivot = new Vector3(GetMousePosition(), 0);
            }
            else if (leftMouse == ButtonState.Pressed && onScreen)
            {
                var mouse = new Vector3(GetMousePosition(), 0);
                Camera.TranslateCamera(mouse - dragPivot);
                dragPivot = mouse;
            }
            else if (leftMouse == ButtonState.Released)
            {
                inDrag = false;
            }
        }
    }
}
