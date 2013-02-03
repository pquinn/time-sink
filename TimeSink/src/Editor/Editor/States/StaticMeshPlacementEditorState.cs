using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Physics;

namespace Editor.States
{
    public class StaticMeshPlacementEditorState : DefaultEditorState
    {
        string textureKey;
        Texture2D texture;

        public StaticMeshPlacementEditorState(Game game, Camera camera, IResourceCache<Texture2D> cache, string textureKey)
            : base(game, camera, cache)
        {
            this.textureKey = textureKey;
        }

        public override void Enter()
        {
            texture = StateMachine.Owner.RenderManager.TextureCache.GetResource(textureKey);
        }

        public override void Execute()
        {
            ScrollCamera();

            if (MouseOnScreen())
            {
                if (InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    var position = new Vector2(
                            InputManager.Instance.CurrentMouseState.X,
                            InputManager.Instance.CurrentMouseState.Y);
                    var tile = new Tile(
                        textureKey,
                        PhysicsConstants.PixelsToMeters(Vector2.Transform(position, Matrix.Invert(Camera.Transform))),
                        0, Vector2.One, RenderLayer.Gameground, .5f);
                    StateMachine.Owner.RegisterTile(tile);

                    StateMachine.RevertToPreviousState(true);
                }
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
