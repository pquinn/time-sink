using System;
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
            for (int i = 0; i < collideables.Count; i++)
            {
                var body = collideables[i];

                foreach (var other in collideables.Skip(i))
                {
                    if (Collided.Invoke(body.CollisionGeometry, other.CollisionGeometry))
                    {
                        OnCollidedWith.Invoke(body, other);
                        OnCollidedWith.Invoke(other, body);
                    }
                }
            }
        }

        [OnCollidedWith.Overload]
        public static void OnCollidedWith(ICollideable a, ICollideable b)
        {
            //Do nothing
        }
    }
}
