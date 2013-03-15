using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Physics
{   
    public struct TimeScaleCircle
    {
        public Microsoft.Xna.Framework.Vector2 Center;
        public float Radius;
        public float TimeScale;

        public float CalcScale(Microsoft.Xna.Framework.Vector2 v)
        {
            var dist = Microsoft.Xna.Framework.Vector2.DistanceSquared(v, Center);
            var r2 = Radius * Radius;
            var ratio = Math.Min(1f, dist / r2);
            //ratio == 0 -> TimeScale
            //ratio == 1 -> 1
            return TimeScale + (1 - TimeScale) * ratio;
        }
    }
}
