using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.Rendering
{
    public class ParentedRendering : IRendering
    {
        public ParentedRendering(IRendering parent, Stack<IRendering> children)
        {
            Parent = parent;
            Children = children;
        }

        public IRendering Parent { get; set; }

        public Stack<IRendering> Children { get; set; }

        public RenderLayer RenderLayer { get; set; }

        public float DepthWithinLayer { get; set; }

        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix transform)
        {
            Parent.Draw(spriteBatch, cache, transform);

            var relativeTransform =
                Matrix.CreateScale(new Vector3(Parent.Position.X, Parent.Position.Y, 1)) *
                Matrix.CreateRotationZ(Parent.Rotation) *
                Matrix.CreateTranslation(Parent.Scale.X, Parent.Scale.Y, 0) *
                transform;

            Children.ForEach(x => x.Draw(spriteBatch, cache, relativeTransform));
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException(); //don't use parented rendering for previews
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException(); //don't use parented rendering for previews
        }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException(); //don't use parented rendering for previews
        }


        public Vector2 Position
        {
            get
            {
                return Parent.Position;
            }
            set
            {
                throw new InvalidOperationException("Can't set the position of a parented rendering");
            }
        }

        public float Rotation
        {
            get
            {
                return Parent.Rotation;
            }
            set
            {
                throw new InvalidOperationException("Can't set the rotation of a parented rendering");
            }
        }

        public Vector2 Scale
        {
            get
            {
                return Parent.Scale;
            }
            set
            {
                throw new InvalidOperationException("Can't set the scale of a parented rendering");
            }
        }
    }
}
