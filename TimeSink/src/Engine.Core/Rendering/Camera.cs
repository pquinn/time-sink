using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;

namespace TimeSink.Engine.Core.Rendering
{
    /// <summary>
    /// Provides basic orthographic camera capabilities.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Creates a camera scaled by the given factor, then translated to the given point.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="position">Translation factor.</param>
        public Camera(Vector2 scale, Vector3 position)
        {
            Transform = Matrix.CreateScale(new Vector3(scale, 1)) * Matrix.CreateTranslation(position);
        }

        public Matrix Transform { get; private set; }

        /// <summary>
        /// Translates the camera by the given amount.
        /// </summary>
        /// <param name="translation">Translation factor.</param>
        public void TranslateCamera(Vector3 translation)
        {
            Transform *= Matrix.CreateTranslation(translation);
        }

        /// <summary>
        /// Moves the camera to the given position.
        /// </summary>
        /// <param name="position">Position to move to.</param>
        public void MoveCameraTo(Vector3 position)
        {
            Transform *= Matrix.CreateTranslation(position - Transform.Translation);
        }

        /// <summary>
        /// Zooms the camera about the current position by the given amount.
        /// </summary>
        /// <param name="zoomAmount">Zoom amount.</param>
        public void ZoomCamera(Vector2 zoomAmount)
        {
            Transform *= Matrix.CreateTranslation(-Transform.Translation);
            Transform *= Matrix.CreateScale(new Vector3(zoomAmount, 1));
            Transform *= Matrix.CreateTranslation(Transform.Translation);
        }

        /// <summary>
        /// Zooms the camera about the given point by the given amount.
        /// </summary>
        /// <param name="zoomAmount">Zoom amount.</param>
        public void ZoomCamera(Vector2 zoomAmount, Vector3 zoomPoint)
        {
            Transform *= Matrix.CreateTranslation(-zoomPoint);
            Transform *= Matrix.CreateScale(new Vector3(zoomAmount, 1));
            Transform *= Matrix.CreateTranslation(zoomPoint);
        }

        public void RevertZoom()
        {
            Transform = Matrix.CreateTranslation(Transform.Translation);
        }
    }
}
