using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Engine.Core.Rendering
{
    public class TextRendering : IRendering
    {
        protected String text;
        Color color;

        public RenderLayer RenderLayer { get; set; }
        private float wtf;
        public float DepthWithinLayer 
        { 
            get { return wtf; } 
            set { wtf = value; } }

        public TextRendering(String text, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            this.text = text;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            this.color = color;
            this.DepthWithinLayer = 0;
            this.RenderLayer = RenderLayer.Gameground;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch, IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            var relativeTransform =
                Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(Position.X, Position.Y, 0) *
                globalTransform;


            spriteBatch.Begin(
                SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                Matrix.Identity);




            spriteBatch.DrawString(EngineGame.Instance.ScreenManager.Font, text, Position, color);
            spriteBatch.End();
        }

        public bool Contains(Vector2 point, IResourceCache<Texture2D> cache, Matrix transform)
        {
            return false;
        }

        public Vector2 GetCenter(IResourceCache<Texture2D> cache, Matrix transform)
        {
            return Vector2.One;
        }

        public NonAxisAlignedBoundingBox GetNonAxisAlignedBoundingBox(IResourceCache<Texture2D> cache, Matrix globalTransform)
        {
            return new NonAxisAlignedBoundingBox();
        }
    }
}
