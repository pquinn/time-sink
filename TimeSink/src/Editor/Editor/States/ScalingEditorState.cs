using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;

namespace Editor.States
{
    public class ScalingEditorState : SelectionEditorState
    {
        private Tuple<Vector2, Vector2> selectedEdge;
        private Vector2 scalingNormal;
        private Vector2 origScale;

        private Vector2 lastMouse;

        public ScalingEditorState(Camera camera, IResourceCache<Texture2D> cache)
            : base(camera, cache)
        {
        }

        protected override void DragStart()
        {
            base.DragStart();

            origScale = selectedMeshes[drillIndex].Scale;

            lastMouse = GetMousePosition();
        }

        protected override void HandleDrag()
        {
            if (InputManager.Instance.Pressed(Keys.B))
            {
                Debugger.Break();
            }

            var mouse = GetMousePosition();
            var dragOffset = lastMouse - mouse;
            selectedMeshes[drillIndex].Expand(
                StateMachine.Owner.RenderManager.TextureCache,
                dragOffset, origScale, Matrix.Identity);

            lastMouse = mouse;
            origScale = selectedMeshes[drillIndex].Scale;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
