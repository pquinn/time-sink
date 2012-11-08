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
                    HandleDrag();
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
                    var accRef = new Rendering.BoundingBox(
                        Single.PositiveInfinity, Single.NegativeInfinity,
                        Single.NegativeInfinity, Single.PositiveInfinity);
                    if (i == drillIndex)
                    {
                        selectedMeshes[i].Rendering.GetBoundingBox(
                            level.RenderManager.TextureCache,
                            ref accRef,
                            new Vector2(0, 0));

                        DrawBoundingBox(spriteBatch, camera, Color.LightGreen, accRef);
                    }
                    else
                    {
                        selectedMeshes[i].Rendering.GetBoundingBox(
                            level.RenderManager.TextureCache,
                            ref accRef,
                            new Vector2(0, 0));

                        DrawBoundingBox(spriteBatch, camera, Color.LightYellow, accRef);
                    }         
            }

            spriteBatch.End();
        }

        private void DrawBoundingBox(SpriteBatch spriteBatch, Camera camera, Color color, Rendering.BoundingBox bounds)
        {
            var blank = StateMachine.Owner.RenderManager.TextureCache.GetResource("blank");
            spriteBatch.DrawRect(
                blank,
                new Vector2(bounds.Min_X, bounds.Min_Y),
                new Vector2(bounds.Max_X, bounds.Max_Y),
                5, color);
        }

        private List<StaticMesh> GetSelections(Level level)
        {
            var selected = new List<StaticMesh>();
            foreach (var mesh in level.GetStaticMeshes())
            {
                if (mesh.Rendering.Contains(
                        GetMousePosition(),
                        level.RenderManager.TextureCache,
                        new Vector2(0, 0)))
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

        private void HandleDrag()
        {
            var mousePos = GetMousePosition();
            var offset = mousePos - lastMouse;
            var newPos = selectedMeshes[drillIndex].Position + offset;
            var offX = 0f;
            var offY = 0f;

            if (EditorProperties.Instance.EnableSnapping)
            {
                var gridSpace = EditorProperties.Instance.GridLineSpacing;
                var halfSpace = gridSpace / 2;
                var leftDist = newPos.X % gridSpace;
                var upDist = newPos.Y % gridSpace;
                offX = (leftDist <= halfSpace) ? -leftDist : halfSpace - leftDist;
                offY = (upDist <= halfSpace) ? -upDist : halfSpace - upDist;
                newPos = new Vector2(newPos.X - offX, newPos.Y - offY);

                lastMouse = mousePos - new Vector2(offX, offY);
                InputManager.ForceMousePosition(lastMouse);
            }
            else
            {
                lastMouse = mousePos;
            }

            selectedMeshes[drillIndex].Position = newPos;
        }
    }
}
