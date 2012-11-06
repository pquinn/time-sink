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
        int drillIndex;

        public SelectionEditorState()
        {
            selectedMeshes = new List<StaticMesh>();
            drillIndex = 0;
        }

        public override void Enter(Level level)
        {
        }

        public override void Execute(Level level)
        {
            if (InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                selectedMeshes = new List<StaticMesh>();
                drillIndex = 0;
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
            else if (InputManager.Instance.IsNewKey(Keys.D) && selectedMeshes.Count > 0)
            {
                drillIndex = (drillIndex + 1) % selectedMeshes.Count;
            }
            else if (InputManager.Instance.Pressed(Keys.Down) && selectedMeshes.Count > 0)
            {
                selectedMeshes[drillIndex].Position += new Vector2(0, 2);
            }
            else if (InputManager.Instance.Pressed(Keys.Up) && selectedMeshes.Count > 0)
            {
                selectedMeshes[drillIndex].Position += new Vector2(0, -2);
            }
            else if (InputManager.Instance.Pressed(Keys.Right) && selectedMeshes.Count > 0)
            {
                selectedMeshes[drillIndex].Position += new Vector2(2, 0);
            }
            else if (InputManager.Instance.Pressed(Keys.Left) && selectedMeshes.Count > 0)
            {
                selectedMeshes[drillIndex].Position += new Vector2(-2, 0);
            }
        }

        public override void Exit(Level level)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera, Level level)
        {
            base.Draw(spriteBatch, camera, level);

            spriteBatch.Begin();

                for (int i = 0; i < selectedMeshes.Count; i++)
                {
                    if (i == drillIndex)
                    {
                        selectedMeshes[i].Rendering.DrawSelected(
                            spriteBatch,
                            level.RenderManager.TextureCache,
                            Color.LightGreen);
                    }
                    else
                    {
                        selectedMeshes[i].Rendering.DrawSelected(
                            spriteBatch,
                            level.RenderManager.TextureCache,
                            Color.LightYellow);
                    }         
            }

            spriteBatch.End();
        }
    }
}
