using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Collisions
{
    public class CollisionSet : ICollisionGeometry
    {
        private HashSet<ICollisionGeometry> _geom = new HashSet<ICollisionGeometry>();
        public HashSet<ICollisionGeometry> Geometry
        {
            get { return _geom; }
        }

        [Collided.Overload]
        public CollisionInfo Collided(ICollisionGeometry cg)
        {
            bool intersected = false;
            float movex = 0, movey = 0;
            float magx = 0, magy = 0;
            CollisionInfo result;
            foreach (var g in _geom)
            {
                result = Collisions.Collided.Invoke(g, cg);
                if (result.Intersect)
                {
                    intersected = true;
                    if (Math.Abs(result.MinimumTranslationVector.X) > magx)
                    {
                        movex = result.MinimumTranslationVector.X;
                        magx = Math.Abs(movex);
                    }
                    if (Math.Abs(result.MinimumTranslationVector.Y) > magy)
                    {
                        movey = result.MinimumTranslationVector.Y;
                        magy = Math.Abs(movey);
                    }
                }
            }
            return new CollisionInfo()
            {
                Intersect = intersected,
                MinimumTranslationVector = new Vector2(movex, movey)
            };
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            foreach (var geom in Geometry)
            {
                geom.Draw(spriteBatch, cache, globalTransform);
            }
        }
    }
}
