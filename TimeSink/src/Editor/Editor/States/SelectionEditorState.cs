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
using TimeSink.Engine.Core.Physics;

namespace Editor.States
{
    public class SelectionEditorState : DefaultEditorState
    {
        private static readonly float KEYBOARD_MOVEMENT_AMOUNT = PhysicsConstants.PixelsToMeters(2);
        private Vector3 cameraOffset = Vector3.Zero;
        private bool tilesOnly;

        protected List<Entity> selectedEntities;
        protected int drillIndex;
        protected bool drag;
        protected bool emptySelect;
        protected Vector2 dragPivot;
        protected Vector2 selectionPivot;

        public SelectionEditorState(Game game, Camera camera, IResourceCache<Texture2D> cache, bool tilesOnly)
            : base(game, camera, cache)
        {
            selectedEntities = new List<Entity>();
            drillIndex = 0;
            this.tilesOnly = tilesOnly;
        }

        public override void Enter()
        {
        }

        public override void Execute()
        {
            ScrollCamera();

            if (MouseOnScreen())
            {
                var buttonState = InputManager.Instance.CurrentMouseState.LeftButton;
                var hasSelect = selectedEntities.Count > 0;
                var lastSelected = hasSelect ? selectedEntities[drillIndex] : null;
                if (buttonState == ButtonState.Pressed && MouseOnScreen())
                {
                    var clicked = GetSelections(StateMachine.Owner);
                    var sameClick = clicked.Contains(lastSelected);
                    if (clicked.Count == 0) // cancel click
                    {
                        emptySelect = true;
                        selectedEntities.Clear();
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
                        selectedEntities = GetSelections(StateMachine.Owner);
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

                if (InputManager.Instance.IsNewKey(Keys.D) && selectedEntities.Count > 0)
                {
                    drillIndex = (drillIndex + 1) % selectedEntities.Count;
                }
                else if (InputManager.Instance.Pressed(Keys.Down) && selectedEntities.Count > 0)
                {
                    selectedEntities[drillIndex].Position += new Vector2(0, KEYBOARD_MOVEMENT_AMOUNT);
                }
                else if (InputManager.Instance.Pressed(Keys.Up) && selectedEntities.Count > 0)
                {
                    selectedEntities[drillIndex].Position += new Vector2(0, -KEYBOARD_MOVEMENT_AMOUNT);
                }
                else if (InputManager.Instance.Pressed(Keys.Right) && selectedEntities.Count > 0)
                {
                    selectedEntities[drillIndex].Position += new Vector2(KEYBOARD_MOVEMENT_AMOUNT, 0);
                }
                else if (InputManager.Instance.Pressed(Keys.Left) && selectedEntities.Count > 0)
                {
                    selectedEntities[drillIndex].Position += new Vector2(-KEYBOARD_MOVEMENT_AMOUNT, 0);
                }
                else if (InputManager.Instance.IsNewKey(Keys.Delete) && selectedEntities.Count > 0)
                {
                    var entity = selectedEntities[drillIndex];
                    if (entity is Tile)
                        StateMachine.Owner.UnregisterTile((Tile)entity);
                    else
                        StateMachine.Owner.UnregisterEntity(entity);
                    selectedEntities.Remove(entity);
                    drillIndex--;
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

            for (int i = 0; i < selectedEntities.Count; i++)
            {
                var box = selectedEntities[i].Preview.GetNonAxisAlignedBoundingBox(
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
            selectionPivot = new Vector2(
                PhysicsConstants.MetersToPixels(selectedEntities[drillIndex].Position.X),
                PhysicsConstants.MetersToPixels(selectedEntities[drillIndex].Position.Y));
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

            selectedEntities[drillIndex].Position = new Vector2(
                PhysicsConstants.PixelsToMeters(newPos.X), 
                PhysicsConstants.PixelsToMeters(newPos.Y));
        }

        private void DrawBoundingBox(SpriteBatch spriteBatch, Camera camera, Color color, NonAxisAlignedBoundingBox box)
        {
            var blank = StateMachine.Owner.RenderManager.TextureCache.GetResource("blank");
            spriteBatch.DrawRect(blank, box, 5, color);
        }

        private List<Entity> GetSelections(LevelManager levelManager)
        {
            var selected = new List<Entity>();
            var entities = tilesOnly ? levelManager.Level.Tiles : levelManager.Level.Tiles.Concat(levelManager.Level.Entities);
            foreach (var entity in entities)
            {
                if (entity.Preview.Contains(
                        GetMousePosition(),
                        levelManager.RenderManager.TextureCache,
                        Camera.Transform))
                {
                    selected.Add(entity);
                }
            }

            return selected;
        }
    }
}
