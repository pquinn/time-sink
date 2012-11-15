using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Editor.States
{
    public class RotationEditorState : SelectionEditorState
    {
        Vector2 center;
        Vector2 dragOffset;
        double origAngle;
        double curAngle;

        protected override void DragStart()
        {
            base.DragStart();

            center = selectedMeshes[drillIndex].Rendering.GetCenter(
                StateMachine.Owner.RenderManager.TextureCache, Matrix.Identity);
            dragOffset = dragPivot - center;

            if (!(dragOffset == Vector2.Zero))
            {
                dragOffset.Normalize();
            }

            origAngle = Math.Atan2(dragOffset.Y, dragOffset.X);
            curAngle = selectedMeshes[drillIndex].Rotation;
        }
        protected override void HandleDrag()
        {
            var dir = GetMousePosition() - center;

            if (InputManager.Instance.Pressed(Keys.V))
            {
                Debugger.Break();
            }

            //var dot = Vector2.Dot(dragOffset, curOffset);
            //var length = dragOffset.Length() * curOffset.Length();

            //var angle = Math.Acos(dot / length);
            if (!(dir == Vector2.Zero))
                dir.Normalize();

            var newAngle = Math.Atan2(dir.Y, dir.X);

            selectedMeshes[drillIndex].Rotation = 
                (float)(curAngle + newAngle - origAngle);
        }
    }
}
