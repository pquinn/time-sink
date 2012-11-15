using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;

namespace TimeSink.Engine.Core.Rendering
{
    public class Camera
    {
        private static readonly Camera singleton = new Camera(Vector3.Zero, Vector2.One);

        public static Camera ZeroedCamera { get { return singleton; } }

        public Camera(Vector3 position, Vector2 scale)
        {
            Position = position;
            Scale = scale;
        }

        public Matrix Transform 
        {
            get
            {
                return Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
                       Matrix.CreateTranslation(Position);
            }
        }
        
        public Vector2 Scale { get; set; }

        public Vector3 Position { get; set; }

        public void PanCamera(Vector3 trans)
        {
            Position += trans;
        }

        public void ZoomCamera(Vector2 scale)
        {
            Scale *= scale;
        }

        public void ResetPosition()
        {
            Position = Vector3.Zero;
        }

        public void ResetScale()
        {
            Scale = Vector2.One;
        }
    }
}
