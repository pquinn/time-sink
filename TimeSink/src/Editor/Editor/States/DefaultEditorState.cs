using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core;

namespace Editor.States
{
    public class DefaultEditorState : State<Level>
    {
        public override void Enter(Level level)
        {            
        }

        public override void Execute(Level level)
        {
        }

        public override void Exit(Level level)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera, Level level)
        {
            level.RenderManager.Draw(spriteBatch);
        }
    }
}
