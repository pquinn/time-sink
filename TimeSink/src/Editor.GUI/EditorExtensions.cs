using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Editor.GUI
{
    public static class EditorExtensions
    {
        public static Vector2 ParseVector(this string s)
        {
            var split = s.Split(',');
            return new Vector2(
                PhysicsConstants.PixelsToMeters((int)Single.Parse(split[0].Trim())),
                PhysicsConstants.PixelsToMeters((int)Single.Parse(split[1].Trim())));
        }

        public static string ToDisplayString(this Vector2 v)
        {
            return string.Format(
                "{0}, {1}", 
                PhysicsConstants.MetersToPixels(v.X), 
                PhysicsConstants.MetersToPixels(v.Y));
        }
    }
}
