using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MechanicsTest.Collisions
{
    delegate void CollisionDelegate(ICollideable other);

    interface ICollideable
    {
        Rectangle GetCollisionRectangle();
        event CollisionDelegate Collided;
    }
}
