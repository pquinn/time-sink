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

namespace Editor.States
{
    public class ScalingEditorState : SelectionEditorState
    {
        private Tuple<Vector2, Vector2> selectedEdge;
        private Vector2 scalingNormal;
        private Vector2 origScale;

        private Vector2 lastMouse;

        protected override void DragStart()
        {
            base.DragStart();

            //Vector2 normal;

            //selectedEdge = selectedMeshes[drillIndex].Rendering.GetEdgeWithinTolerance(
            //    GetMousePosition(), 30,
            //    StateMachine.Owner.RenderManager.TextureCache, Matrix.Identity,
            //    out normal);

            origScale = selectedMeshes[drillIndex].Scale;
            //scalingNormal = normal;
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
            //if (selectedEdge != null)
            //{
            //    var dragOffset = GetMousePosition() - dragPivot;
            //    var dragDistanceAlongNorm = Vector2.Dot(
            //        dragOffset,
            //        (selectedEdge.Item2 - selectedEdge.Item1).GetSurfaceNormal());

            //    selectedMeshes[drillIndex].Expand(dragDistanceAlongNorm, scalingNormal, 
            //        StateMachine.Owner.RenderManager.TextureCache);
            //}
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera, Level level)
        {
            base.Draw(spriteBatch, camera, level);

            //spriteBatch.Begin();

            //if (selectedEdge != null)
            //{
            //    spriteBatch.DrawLine(
            //        StateMachine.Owner.RenderManager.TextureCache.GetResource("blank"),
            //        selectedEdge.Item1, selectedEdge.Item2, 5, Color.LightBlue);
            //}

            //spriteBatch.End();
        }
    }
}
