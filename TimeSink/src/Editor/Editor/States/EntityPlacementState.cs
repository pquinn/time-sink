using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace Editor.States
{
    public class StaticMeshPlacementEditorState : DefaultEditorState
    {
        string textureKey;
        Texture2D texture;

        public StaticMeshPlacementEditorState(Camera camera, IResourceCache<Texture2D> cache, string textureKey)
            : base(camera, cache)
        {
            this.textureKey = textureKey;
        }

        public override void Enter()
        {

            texture = StateMachine.Owner.RenderManager.TextureCache.GetResource(textureKey);
        }

        public override void Execute()
        {
            if (InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                var mesh = new Tile(
                    textureKey,
                    new Vector2(
                        InputManager.Instance.CurrentMouseState.X,
                        InputManager.Instance.CurrentMouseState.Y),
                    0, Vector2.One,
                    StateMachine.Owner.RenderManager.TextureCache);
                StateMachine.Owner.RegisterStaticMesh(mesh);

                StateMachine.RevertToPreviousState(true);
            }
        }

        public override void Exit()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin();
            
            spriteBatch.Draw(
                texture,
                new Vector2(
                    InputManager.Instance.CurrentMouseState.X,
                    InputManager.Instance.CurrentMouseState.Y),
                new Color(255, 255, 255, 80));

            spriteBatch.End();
        }
    }
}
