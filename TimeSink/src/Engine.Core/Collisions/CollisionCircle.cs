using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core.Collisions
{
    public struct CollisionCircle : ICollisionGeometry
    {
        public Circle Circ;

        public CollisionCircle(Circle c)
        {
            Circ = c;
        }

        [Collided.Overload]
        public CollisionInfo Collided(CollisionCircle c)
        {
            return new CollisionInfo()
            {
                Intersect = c.Circ.Intersects(Circ),
                MinimumTranslationVector = 
                    (c.Circ.Center - this.Circ.Center) * 
                    ((float)Math.Pow(c.Circ.Radius + Circ.Radius, 2) 
                        - Vector2.DistanceSquared(c.Circ.Center, Circ.Center))
            };
        }

        [Collided.Overload]
        public CollisionInfo Collided(AACollisionRectangle r)
        {
            return r.Collided(this);
        }

        //[Collided.Overload]
        //public CollisionInfo Collided(CollisionRectangle r)
        //{
        //    var BA = r.BottomLeft - r.TopLeft;
        //    BA.Normalize();
        //    var BAp = new Vector2(-BA.Y, BA.X);
        //    var CB = r.BottomRight - r.BottomLeft;
        //    CB.Normalize();
        //    var CBp = new Vector2(-CB.Y, CB.X);
        //    var DC = r.TopRight - r.BottomRight;
        //    DC.Normalize();
        //    var DCp = new Vector2(-DC.Y, DC.X);
        //    var AD = r.TopLeft - r.TopRight;
        //    AD.Normalize();
        //    var ADp = new Vector2(-AD.Y, AD.X);

        //    var rect = new CollisionRectangle(
        //        Circ.Center + Circ.Radius * (ADp + AD),
        //        Circ.Center + Circ.Radius * (BAp + BA),
        //        Circ.Center + Circ.Radius * (CBp + CB),
        //        Circ.Center + Circ.Radius * (DCp + DC)
        //    );

        //    return r.Collided(rect);
        //}

        [Collided.Overload]
        public CollisionInfo Collided(IPolygon p)
        {
            //var edges = t.Edges;

            //var BA = edges[0];
            //BA.Normalize();
            //var BAp = new Vector2(-BA.Y, BA.X);
            //var CB = edges[1];
            //CB.Normalize();
            //var CBp = new Vector2(-CB.Y, CB.X);
            //var AC = edges[2];
            //AC.Normalize();
            //var ACp = new Vector2(-AC.Y, AC.X);

            //var tri = new CollisionTriangle(
            //    Circ.Center + Circ.Radius * (ACp + AC),
            //    Circ.Center + Circ.Radius * (BAp + BA),
            //    Circ.Center + Circ.Radius * (CBp + CB)
            //);

            return SeparatingAxisTheorem.PolygonCollision(p, makeBoundingPoly(p));
        }

        [Collided.Overload]
        public CollisionInfo Collided(CollisionSet s)
        {
            return s.Collided(this);
        }

        private Polygon makeBoundingPoly(IPolygon source)
        {
            var vertices = new List<Vector2>();

            var edges = source.Edges;

            foreach (var edge in edges)
            {
                edge.Normalize();
                var perp = new Vector2(-edge.Y, edge.X);
                vertices.Add(edge + perp);
            }

            return new Polygon(vertices.Skip(vertices.Count-1).Concat(vertices.Take(vertices.Count-1)).ToList());
        }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            Vector3 trans;
            Quaternion rot;
            Vector3 scale;

            globalTransform.Decompose(out scale, out rot, out trans);

           spriteBatch.DrawCircle(
               cache, 
               Circ.Center + trans.ToVector2(), 
               new Vector2(
                   Circ.Radius * scale.X,
                   Circ.Radius * scale.Y), 
               Color.Red);
        }
    }
}
