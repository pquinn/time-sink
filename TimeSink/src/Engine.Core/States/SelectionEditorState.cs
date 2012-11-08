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
        bool drag;
        bool emptySelect;
        Vector2 lastMouse;

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
            var buttonState = InputManager.Instance.CurrentMouseState.LeftButton;
            var hasSelect = selectedMeshes.Count > 0;
            var lastSelected = hasSelect ? selectedMeshes[drillIndex] : null;
            if (buttonState == ButtonState.Pressed)
            {
                var clicked = GetSelections(level);
                var sameClick = clicked.Contains(lastSelected);
                if (clicked.Count == 0)
                {
                    emptySelect = true;
                    selectedMeshes.Clear();
                }
                else if (hasSelect && sameClick && !drag && !emptySelect)
                {
                    drag = true;
                    lastMouse = GetMousePosition();
                }
                else if (hasSelect && drag && !emptySelect)
                {
                    var offset = GetMousePosition() - lastMouse;

                    selectedMeshes[drillIndex].Position += offset;

                    lastMouse = GetMousePosition();
                }
                else if (!emptySelect)
                {
                    selectedMeshes = GetSelections(level);
                    drillIndex = 0;
                }
            }
            else if (buttonState == ButtonState.Released)
            {
                if (drag)
                {
                    drag = false;
                }

                emptySelect = false;
            }
            
            if (InputManager.Instance.IsNewKey(Keys.D) && selectedMeshes.Count > 0)
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

        private List<StaticMesh> GetSelections(Level level)
        {
            var selected = new List<StaticMesh>();
            foreach (var mesh in level.GetStaticMeshes())
            {
                if (mesh.Rendering.Contains(
                    GetMousePosition(),
                    level.RenderManager.TextureCache))
                {
                    selected.Add(mesh);
                }
            }

            return selected;
        }

        private Vector2 GetMousePosition()
        {
            return new Vector2(
                    InputManager.Instance.CurrentMouseState.X,
                    InputManager.Instance.CurrentMouseState.Y);
        }
    }
}
