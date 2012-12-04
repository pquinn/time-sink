using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Collisions
{
    public struct WorldCollisionGeometrySegment
    {
        public Vector2 EndPoint;
        public bool IsOneWay;

        public WorldCollisionGeometrySegment(Vector2 end, bool oneWay)
        {
            EndPoint = end;
            IsOneWay = oneWay;
        }
    }
}
