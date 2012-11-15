using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core;

namespace Editor.States
{
    public class SelectionEditorState : DefaultEditorState
    {
        protected List<StaticMesh> selectedMeshes;
        protected int drillIndex;
        protected bool drag;
        protected bool emptySelect;
        protected Vector2 dragPivot;
        protected Vector2 selectionPivot;

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
                if (clicked.Count == 0) // cancel click
                {
                    emptySelect = true;
                    selectedMeshes.Clear();
                }
                else if (hasSelect && sameClick && !drag && !emptySelect)  // enter drag
                {
                    DragStart();
                }
                else if (hasSelect && drag && !emptySelect) // dragging
                {
                    HandleDrag();
                    //lastMouse = GetMousePosition();
                }
                else if (!emptySelect) // basic selection
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
                var box = selectedMeshes[i].Rendering.GetNonAxisAlignedBoundingBox(
                        level.RenderManager.TextureCache,
                        Matrix.Identity);

                if (i == drillIndex)
                {
                    DrawBoundingBox(spriteBatch, camera, Color.LightGreen, box);
                }
                else
                {
                    DrawBoundingBox(spriteBatch, camera, Color.LightYellow, box);
                }
            }

            spriteBatch.End();
        }

        protected virtual void DragStart()
        {
            drag = true;

            dragPivot = GetMousePosition();
            selectionPivot = selectedMeshes[drillIndex].Position;
        }

        protected virtual void HandleDrag()
        {
            var mousePos = GetMousePosition();
            var offset = mousePos - dragPivot;
            var newPos = selectionPivot + offset;
            var offX = 0f;
            var offY = 0f;

            if (EditorProperties.Instance.EnableSnapping && offset.Length() > 0)
            {
                var gridSpace = EditorProperties.Instance.GridLineSpacing;
                var halfSpace = gridSpace / 2;
                var leftDist = newPos.X % gridSpace;
                var upDist = newPos.Y % gridSpace;
                offX = (leftDist <= halfSpace) ? -leftDist : gridSpace - leftDist;
                offY = (upDist <= halfSpace) ? -upDist : gridSpace - upDist;
                newPos = new Vector2(newPos.X + offX, newPos.Y + offY);
            }

            selectedMeshes[drillIndex].Position = newPos;
        }

        protected Vector2 GetMousePosition()
        {
            return new Vector2(
                    InputManager.Instance.CurrentMouseState.X,
                    InputManager.Instance.CurrentMouseState.Y);
        }

        private void DrawBoundingBox(SpriteBatch spriteBatch, Camera camera, Color color, NonAxisAlignedBoundingBox box)
        {
            var blank = StateMachine.Owner.RenderManager.TextureCache.GetResource("blank");
            spriteBatch.DrawRect(blank, box, 5, color);
        }

        private List<StaticMesh> GetSelections(Level level)
        {
            var selected = new List<StaticMesh>();
            foreach (var mesh in level.GetStaticMeshes())
            {
                if (mesh.Rendering.Contains(
                        GetMousePosition(),
                        level.RenderManager.TextureCache,
                        Matrix.Identity))
                {
                    selected.Add(mesh);
                }
            }

            return selected;
        }
    }
}
