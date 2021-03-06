﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace MechanicsTest.Collisions
{
    public interface ICollideable
    {
        ICollisionGeometry CollisionGeometry { get; }
    }

    public class CollisionManager
    {
        private HashSet<ICollideable> collideables = new HashSet<ICollideable>();

        public bool RegisterCollisionBody(ICollideable coll)
        {
            return collideables.Add(coll);
        }

        public bool UnregisterPhysicsBody(ICollideable coll)
        {
            return collideables.Remove(coll);
        }

        public void Update(GameTime gt)
        {
            int i = 1;
            foreach (var body in collideables)
            {
                foreach (var other in collideables.Skip(i))
                {
                    if (Collided.Invoke(body.CollisionGeometry, other.CollisionGeometry))
                    {
                        Collisions.OnCollidedWith.Invoke(body, other);
                        Collisions.OnCollidedWith.Invoke(other, body);
                    }
                }
                i++;
            }
        }
    }
}
