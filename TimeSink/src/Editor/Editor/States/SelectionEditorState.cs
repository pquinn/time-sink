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
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.States;

namespace Editor.States
{
    public class SelectionEditorState : DefaultEditorState
    {
        const int CAMERA_TOLERANCE = 10;
        const int CAMERA_MOVE_SPEED = 5;
        private Vector3 cameraOffset = Vector3.Zero;

        protected List<Tile> selectedMeshes;
        protected int drillIndex;
        protected bool drag;
        protected bool emptySelect;
        protected Vector2 dragPivot;
        protected Vector2 selectionPivot;

        public SelectionEditorState(Camera camera, IResourceCache<Texture2D> cache)
            : base(camera, cache)
        {
            selectedMeshes = new List<Tile>();
            drillIndex = 0;
        }

        public override void Enter()
        {
        }

        public override void Execute()
        {
            Console.WriteLine(GetMousePosition());
            var buttonState = InputManager.Instance.CurrentMouseState.LeftButton;
            var hasSelect = selectedMeshes.Count > 0;
            var lastSelected = hasSelect ? selectedMeshes[drillIndex] : null;
            if (buttonState == ButtonState.Pressed && MouseOnScreen())
            {
                var clicked = GetSelections(StateMachine.Owner);
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
                    selectedMeshes = GetSelections(StateMachine.Owner);
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

            cameraOffset = Vector3.Zero;
            var mouse = GetMousePosition();
            if (mouse.X < CAMERA_TOLERANCE && mouse.X > 0)
                cameraOffset = -Vector3.UnitX * CAMERA_MOVE_SPEED;
            if (mouse.X > Constants.SCREEN_X - CAMERA_TOLERANCE && mouse.X < Constants.SCREEN_X)
                cameraOffset = Vector3.UnitX * CAMERA_MOVE_SPEED;
            if (mouse.Y < CAMERA_TOLERANCE && mouse.Y > 0)
                cameraOffset = -Vector3.UnitY * CAMERA_MOVE_SPEED;
            if (mouse.Y > Constants.SCREEN_Y - CAMERA_TOLERANCE && mouse.Y < Constants.SCREEN_Y)
                cameraOffset = Vector3.UnitY * CAMERA_MOVE_SPEED;
        }

        public override void Exit()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Camera.PanCamera(cameraOffset);

            base.Draw(spriteBatch);

            spriteBatch.Begin();

            for (int i = 0; i < selectedMeshes.Count; i++)
            {
                var box = selectedMeshes[i].Rendering.GetNonAxisAlignedBoundingBox(
                        StateMachine.Owner.RenderManager.TextureCache,
                        Camera.Transform);

                if (i == drillIndex)
                {
                    DrawBoundingBox(spriteBatch, Camera, Color.LightGreen, box);
                }
                else
                {
                    DrawBoundingBox(spriteBatch, Camera, Color.LightYellow, box);
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

        private void DrawBoundingBox(SpriteBatch spriteBatch, Camera camera, Color color, NonAxisAlignedBoundingBox box)
        {
            var blank = StateMachine.Owner.RenderManager.TextureCache.GetResource("blank");
            spriteBatch.DrawRect(blank, box, 5, color);
        }

        private List<Tile> GetSelections(LevelManager levelManager)
        {
            var selected = new List<Tile>();
            foreach (var tile in levelManager.Level.Tiles)
            {
                if (tile.Rendering.Contains(
                        GetMousePosition(),
                        levelManager.RenderManager.TextureCache,
                        Camera.Transform))
                {
                    selected.Add(tile);
                }
            }

            return selected;
        }
    }
}
