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
            foreach (var body in bodies)
            {
                foreach (var other in bodies)
                {
                    if (body != other)
                    {
                        if (Collided.Invoke(body, other))
                        {
                            OnCollidedWith.Invoke(body, other)
                        }
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
