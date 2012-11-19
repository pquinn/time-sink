using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Input;

namespace Editor.States
{
    public class DefaultEditorState : State<LevelManager>
    {
        public DefaultEditorState(Camera camera, IResourceCache<Texture2D> textureCache)
        {
            Camera = camera;
            TextureCache = textureCache;
        }

        protected Camera Camera { get; set; }

        protected IResourceCache<Texture2D> TextureCache { get; set; }

        public override void Enter()
        {            
        }

        public override void Execute()
        {
        }

        public override void Exit()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            StateMachine.Owner.RenderManager.Draw(spriteBatch, Camera);
        }

        protected Vector2 GetMousePosition()
        {
            return new Vector2(
                    InputManager.Instance.CurrentMouseState.X,
                    InputManager.Instance.CurrentMouseState.Y);
        }
    }
}
