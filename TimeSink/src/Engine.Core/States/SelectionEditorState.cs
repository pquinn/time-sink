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
        List<StaticMesh> selectedMeshes;

        public SelectionEditorState()
        {
            selectedMeshes = new List<StaticMesh>();
        }

        public override void Enter(Level level)
        {
        }

        public override void Execute(Level level)
        {
            if (InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var mesh in level.GetStaticMeshes())
                {
                    if (mesh.Rendering.Contains(
                        new Vector2(
                            InputManager.Instance.CurrentMouseState.X,
                            InputManager.Instance.CurrentMouseState.Y),
                        level.RenderManager.TextureCache))
                    {
                        selectedMeshes.Add(mesh);
                    }
                }
            }
        }

        public override void Exit(Level level)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera, Level level)
        {
            base.Draw(spriteBatch, camera, level);

            spriteBatch.Begin();

            if (selectedMeshes.Count > 0)
            {
                var mesh = selectedMeshes[0];
                mesh.Rendering.DrawSelected(spriteBatch, level.RenderManager.TextureCache);
            }

            spriteBatch.End();
        }
    }
}
