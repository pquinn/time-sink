using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Core.Particles
{
    public class Particle : IRenderable
    {
        public Vector2 StartingPos;
        public Vector2 Position;
        Vector2 StartDirection;
        Vector2 EndDirection;
        float LifeLeft;
        float StartingLife;
        float ScaleBegin;
        float ScaleEnd;
        Color StartColor;
        Color EndColor;
        float lifePhase;
        private string texture;
        private IResourceCache<Texture2D> textureCache;
        private float MaxWidth;
        private float MaxHeight;

        public Particle(Vector2 Position, Vector2 StartDirection, Vector2 EndDirection, float StartingLife, float ScaleBegin, float ScaleEnd, Color StartColor, Color EndColor, string texture, IResourceCache<Texture2D> textureCache, float MaxWidth, float MaxHeight)
        {
            this.Position = Position;
            this.StartingPos = Position;
            this.StartDirection = StartDirection;
            this.EndDirection = EndDirection;
            this.StartingLife = StartingLife;
            this.LifeLeft = StartingLife;
            this.ScaleBegin = ScaleBegin;
            this.ScaleEnd = ScaleEnd;
            this.StartColor = StartColor;
            this.EndColor = EndColor;
            this.texture = texture;
            this.textureCache = textureCache;
            this.MaxWidth = MaxWidth;
            this.MaxHeight = MaxHeight;
        }

        public bool Update(GameTime time, EngineGame world)
        {
            LifeLeft -= time.ElapsedGameTime.Milliseconds;
            if (LifeLeft <= 0)
            {
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                return false;
            }
            if (Position.X >= (StartingPos.X + MaxWidth) || Position.X <= (StartingPos.X - MaxWidth) ||
                Position.Y >= (StartingPos.Y + MaxHeight) || Position.Y <= (StartingPos.Y - MaxHeight))
            {
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                return false;
            }
            lifePhase = LifeLeft / StartingLife;      // 1 means newly created 0 means dead.
            Position += MathLib.LinearInterpolate(EndDirection, StartDirection, lifePhase) * time.ElapsedGameTime.Milliseconds;
            return true;
        }

        public System.Collections.Generic.List<IRendering> Renderings
        {
            get
            {
                float currScale = MathLib.LinearInterpolate(ScaleEnd, ScaleBegin, lifePhase);
                return new List<IRendering>() 
                {
                    new BasicRendering(texture)
                    {
                        Position = PhysicsConstants.MetersToPixels(this.Position),
                        Scale = Vector2.One,
                        TintColor = StartColor,
                        DepthWithinLayer = -50
                    }
                };
            }
        }

        public string EditorName
        {
            get { throw new NotImplementedException(); }
        }

        public IRendering Preview
        {
            get { throw new NotImplementedException(); }
        }
    }
}