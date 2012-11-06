using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core.States
{
    public class SelectionEditorState : DefaultEditorState
    {
        Texture2D texture;

        public SelectionEditorState()
        {
        }

        public override void Enter(Level level)
        {
        }

        public override void Execute(Level level)
        {
            if (InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
            }
        }

        public override void Exit(Level level)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera, Level l)
        {
            base.Draw(spriteBatch, camera, l);
        }
    }
}
