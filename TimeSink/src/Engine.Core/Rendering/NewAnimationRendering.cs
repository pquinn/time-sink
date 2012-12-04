using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class NewAnimationRendering : IRendering
    {
        private string spriteSheet;
        private Vector2 srcRectSize;

        private List<BasicRendering> frames;

        public NewAnimationRendering(string spriteSheet, Vector2 srcRectSize, int numFrames, 
            Vector2 position, float rotation, Vector2 scale)
        {
            this.spriteSheet = spriteSheet;
            this.srcRectSize = srcRectSize;
            this.NumFrames = numFrames;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            this.CurrentFrame = 0;
            this.frames = InitRenderings();
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public int CurrentFrame { get; set; }
        public int NumFrames { get; set; }

        private List<BasicRendering> InitRenderings()
        {
            var renderings = new List<BasicRendering>();

            for (var i = 0; i < NumFrames; i++)
            {
                renderings.Add(
                    new BasicRendering(
                        spriteSheet, Vector2.Zero, 0, Vector2.One,
                        new Rectangle(
                            i * (int)srcRectSize.X, 
                            0, 
                            (int)srcRectSize.X, 
                            (int)srcRectSize.Y)));
            }

            return renderings;
        }
        
        public void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix transform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(Scale, 1)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(new Vector3(Position, 0)) *
                transform;
            
            frames[CurrentFrame].Draw(spriteBatch, cache, relativeTransform);
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Vector2 point, Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }

        public Vector2 GetCenter(Caching.IResourceCache<Microsoft.Xna.Framework.Graphics.Texture2D> cache, Matrix globalTransform)
        {
            throw new NotImplementedException();
        }
    }
}
