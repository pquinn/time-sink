using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Entities.Objects;

namespace TimeSink.Engine.Core.Particles
{
    public class Particle : IRenderable
    {
        public Vector2 Position;
        Vector2 StartDirection;
        Vector2 EndDirection;
        float LifeLeft;
        float StartingLife;
        float ScaleBegin;
        float ScaleEnd;
        Color StartColor;
        Color EndColor;
        Emitter Parent;
        float lifePhase;
        private string texture;
        private IResourceCache<Texture2D> textureCache;

        public Particle(Vector2 Position, Vector2 StartDirection, Vector2 EndDirection, float StartingLife, float ScaleBegin, float ScaleEnd, Color StartColor, Color EndColor, Emitter Yourself, string texture, IResourceCache<Texture2D> textureCache)
        {
            this.Position = Position;
            this.StartDirection = StartDirection;
            this.EndDirection = EndDirection;
            this.StartingLife = StartingLife;
            this.LifeLeft = StartingLife;
            this.ScaleBegin = ScaleBegin;
            this.ScaleEnd = ScaleEnd;
            this.StartColor = StartColor;
            this.EndColor = EndColor;
            this.Parent = Yourself;
            this.texture = texture;
            this.textureCache = textureCache;
        }

        public bool Update(GameTime time)
        {
            LifeLeft -= time.ElapsedGameTime.Seconds;
            if (LifeLeft <= 0)
                return false;
            lifePhase = LifeLeft / StartingLife;      // 1 means newly created 0 means dead.
            Position += MathLib.LinearInterpolate(EndDirection, StartDirection, lifePhase) * time.ElapsedGameTime.Seconds;
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
                        Scale = BasicRendering.CreateScaleFromSize((int)currScale, (int)currScale, texture, textureCache),
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