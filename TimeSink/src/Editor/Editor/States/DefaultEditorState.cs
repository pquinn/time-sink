using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Input;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Forms;

namespace Editor.States
{
    public class DefaultEditorState : State<LevelManager>
    {
        const int CAMERA_TOLERANCE = 10;
        const int CAMERA_MOVE_SPEED = 5;

        public DefaultEditorState(Game game, Camera camera, IResourceCache<Texture2D> textureCache)
        {
            Game = game;
            Camera = camera;
            TextureCache = textureCache;
        }

        public bool IsMouseInteractionEnabled { get; set; }

        protected Game Game { get; set; }

        protected Camera Camera { get; set; }

        protected IResourceCache<Texture2D> TextureCache { get; set; }

        public override void Enter()
        {            
        }

        public override void Execute()
        {
        }

        public override void Exit()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            StateMachine.Owner.EditorRenderManager.Draw(spriteBatch, Camera);
        }

        protected Vector2 GetMousePosition()
        {
            return new Vector2(
                    InputManager.Instance.CurrentMouseState.X,
                    InputManager.Instance.CurrentMouseState.Y);
        }

        protected bool MouseOnScreen()
        {
            var mouse = GetMousePosition();

            var isActive = Game.IsActive;
            return isActive &&
                   mouse.X > 0 && mouse.X < Game.GraphicsDevice.Viewport.Width &&
                   mouse.Y > 0 && mouse.Y < Game.GraphicsDevice.Viewport.Height;
        }

        protected void ScrollCamera()
        {
            var cameraOffset = Vector3.Zero;
            var mouse = GetMousePosition();
            if (mouse.X < CAMERA_TOLERANCE && mouse.X > 0)
                cameraOffset = -Vector3.UnitX * CAMERA_MOVE_SPEED;
            if (mouse.X > Game.GraphicsDevice.Viewport.Width - CAMERA_TOLERANCE && mouse.X < Game.GraphicsDevice.Viewport.Width)
                cameraOffset = Vector3.UnitX * CAMERA_MOVE_SPEED;
            if (mouse.Y < CAMERA_TOLERANCE && mouse.Y > 0)
                cameraOffset = -Vector3.UnitY * CAMERA_MOVE_SPEED;
            if (mouse.Y > Game.GraphicsDevice.Viewport.Height - CAMERA_TOLERANCE && mouse.Y < Game.GraphicsDevice.Viewport.Height)
                cameraOffset = Vector3.UnitY * CAMERA_MOVE_SPEED;
            
            Camera.PanCamera(cameraOffset);
        }
    }
}
