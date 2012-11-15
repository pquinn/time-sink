﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Collisions
{
    public class CollisionManager
    {
        private HashSet<ICollideable> collideables = new HashSet<ICollideable>();

        public bool RegisterCollisionBody(ICollideable coll)
        {
            return collideables.Add(coll);
        }

        public bool UnregisterCollisionBody(ICollideable coll)
        {
            return collideables.Remove(coll);
        }

        public void Update(GameTime gt)
        {
            CollisionInfo result;

            int i = 1;
            foreach (var body in collideables)
            {
                foreach (var other in collideables.Skip(i))
                {
                    result = Collided.Invoke(body.CollisionGeometry, other.CollisionGeometry);
                    if (result.Intersect)
                    {
                        Collisions.OnCollidedWith.Invoke(body, other, result);
                        Collisions.OnCollidedWith.Invoke(other, body, result);
                    }
                }
                i++;
            }
        }

        public void Initialize()
        {
            Collided.DoAutoRegister();
            OnCollidedWith.DoAutoRegister();
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            spriteBatch.Begin();

            foreach (var collideable in collideables)
            {
                collideable.CollisionGeometry.Draw(spriteBatch, cache, globalTransform);
            }

            spriteBatch.End();
        }
    }
}
