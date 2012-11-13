using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace TimeSink.Engine.Core.States
{
    public class StaticMeshPlacementEditorState : DefaultEditorState
    {
        string textureKey;
        Texture2D texture;

        public StaticMeshPlacementEditorState(string textureKey)
        {
            this.textureKey = textureKey;
        }

        public override void Enter(Level level)
        {

            texture = StateMachine.Owner.RenderManager.TextureCache.GetResource(textureKey);
        }

        public override void Execute(Level level)
        {
            if (InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                var mesh = new StaticMesh(
                    textureKey,
                    new Vector2(
                        Input.InputManager.Instance.CurrentMouseState.X - (texture.Width / 2),
                        Input.InputManager.Instance.CurrentMouseState.Y - (texture.Height / 2)),
                    0, Vector2.One);
                level.RegisterStaticMesh(mesh);

                StateMachine.RevertToPreviousState(true);
            }
        }

        public override void Exit(Level level)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera, Level l)
        {
            base.Draw(spriteBatch, camera, l);

            spriteBatch.Begin();

            Console.WriteLine(
                "{0}, {1}",
                Input.InputManager.Instance.CurrentMouseState.X,
                Input.InputManager.Instance.CurrentMouseState.Y);
            spriteBatch.Draw(
                texture,
                new Vector2(
                    Input.InputManager.Instance.CurrentMouseState.X - (texture.Width / 2),
                    Input.InputManager.Instance.CurrentMouseState.Y - (texture.Height / 2)),
                new Color(255, 255, 255, 80));

            spriteBatch.End();
        }
    }
}
