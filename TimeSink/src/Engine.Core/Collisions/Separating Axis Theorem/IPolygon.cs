using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core.Collisions
{
    public interface IPolygon : ICollisionGeometry
    {
        Vector2 Center { get; }
        IList<Vector2> Edges { get; }
        IList<Vector2> Vertices { get; }
    }

    public abstract class APolygon : IPolygon
    {
        public virtual Vector2 Center
        {
            get 
            {
                Vector2 result = Vector2.Zero;
                foreach (var vertex in Vertices)
                    result += vertex;
                return result / Vertices.Count;
            }
        }

        public virtual IList<Vector2> Edges
        {
            get
            {
                List<Vector2> result = new List<Vector2>();
                foreach (var pair in Vertices.Zip(Vertices.Skip(1).Concat(Vertices.Take(1)), Tuple.Create))
                    result.Add(pair.Item1 - pair.Item2);
                return result;
            }
        }

        public abstract IList<Vector2> Vertices { get; }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var transformedVerts = Vertices.Select(
                    v => Vector2.Transform(v, globalTransform));

            for (int i = 0; i < Vertices.Count; i++)
            {
                spriteBatch.DrawLine(
                    cache.GetResource("blank"), 
                    Vertices[i], 
                    Vertices[(i + 1) % Vertices.Count], 
                    3, 
                    Color.Red);
            }
        }
    }

    internal class Polygon : APolygon
    {
        private IList<Vector2> _vertices;
        public override IList<Vector2> Vertices
        {
            get { return _vertices; }
        }

        public Polygon(IList<Vector2> verts)
        {
            _vertices = verts;
        }
    }
}
