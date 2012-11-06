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
        Vector2 translation;
        Point worldSize;

        public Camera()
        {
            translation = new Vector2(0, 0);
        }

        public Vector2 Translation
        {
            get { return translation; }
        }

        public void PanCamera(Vector2 trans)
        {
            translation = new Vector2(Math.Min(translation.X - trans.X, 0),
                                      Math.Min(translation.Y - trans.Y, 0));
        }
    }
}
