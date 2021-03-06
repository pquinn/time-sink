﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Particles;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Objects
{

    public class Emitter : Entity
    {
        public Vector2 RelPosition;                    // Position relative to collection.
        public int Budget;                      // Max number of alive particles.
        float NextSpawnIn;                      // This is a random number generated using the SecPerSpawn.
        float MSecPassed;                        // Time pased since last spawn.
        LinkedList<Particle> ActiveParticles;   // A list of all the active particles.
        public string ParticleSprite;        // This is what the particle looks like.
        public Random random;                          // Pointer to a random object passed trough constructor.

        public Vector2 SecPerSpawn;
        public Vector2 SpawnDirection;
        public Vector2 SpawnNoiseAngle;
        public Vector2 StartLife;
        public Vector2 StartScale;
        public Vector2 EndScale;
        public Color StartColor1;
        public Color StartColor2;
        public Color EndColor1;
        public Color EndColor2;
        public Vector2 StartSpeed;
        public Vector2 EndSpeed;
        public float MaxWidth;
        public float MaxHeight;
        private Vector2 centerPoint;

        public Emitter(Vector2 SecPerSpawn, Vector2 SpawnDirection, Vector2 SpawnNoiseAngle, Vector2 StartLife, Vector2 StartScale,
                    Vector2 EndScale, Color StartColor1, Color StartColor2, Color EndColor1, Color EndColor2, Vector2 StartSpeed,
                    Vector2 EndSpeed, int Budget, Vector2 RelPosition, string ParticleSprite, Random random, Vector2 position, float maxWidth, float maxHeight)
        {
            this.SecPerSpawn = SecPerSpawn;
            this.SpawnDirection = SpawnDirection;
            this.SpawnNoiseAngle = SpawnNoiseAngle;
            this.StartLife = StartLife;
            this.StartScale = StartScale;
            this.EndScale = EndScale;
            this.StartColor1 = StartColor1;
            this.StartColor2 = StartColor2;
            this.EndColor1 = EndColor1;
            this.EndColor2 = EndColor2;
            this.StartSpeed = StartSpeed;
            this.EndSpeed = EndSpeed;
            this.Budget = Budget;
            this.RelPosition = RelPosition;
            this.ParticleSprite = ParticleSprite;
            this.random = random;
            ActiveParticles = new LinkedList<Particle>();
            this.NextSpawnIn = MathLib.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
            this.MSecPassed = 0.0f;
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;
            Position = position;
        }

        public Emitter(Vector2 SecPerSpawn, Vector2 SpawnDirection, Vector2 SpawnNoiseAngle, Vector2 StartLife, Vector2 StartScale,
            Vector2 EndScale, Color StartColor1, Color StartColor2, Color EndColor1, Color EndColor2, Vector2 StartSpeed,
            Vector2 EndSpeed, int Budget, Vector2 RelPosition, string ParticleSprite, Random random, Vector2 position, float maxWidth, float maxHeight, Vector2 centerPoint)
            :this(SecPerSpawn, SpawnDirection, SpawnNoiseAngle, StartLife, StartScale, EndScale, StartColor1, StartColor2, EndColor1, EndColor2,
                StartSpeed, EndSpeed, Budget, RelPosition, ParticleSprite, random, position, maxWidth, maxHeight)
        {
            this.centerPoint = centerPoint;
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            MSecPassed += time.ElapsedGameTime.Milliseconds;
            while (MSecPassed > NextSpawnIn)
            {
                var rand = new Random().Next(-(int)MaxWidth, (int)MaxWidth);
                var newRand = rand * new Random().NextDouble() * 1.5;
                if (ActiveParticles.Count < Budget)
                {
                    var pos = RelPosition + new Vector2(PhysicsConstants.PixelsToMeters((float)newRand), 
                                                        PhysicsConstants.PixelsToMeters((float)new Random().Next(-(int)MaxHeight / 5,(int)MaxHeight / 5))) + Position;
                    // Spawn a particle
                    Vector2 StartDirection = Vector2.Zero;
                    if (centerPoint == Vector2.Zero)
                    {
                        StartDirection = Vector2.Transform(SpawnDirection, Matrix.CreateRotationZ(MathLib.LinearInterpolate(SpawnNoiseAngle.X, SpawnNoiseAngle.Y, random.NextDouble())));
                        StartDirection.Normalize();
                    }
                    else
                    {
                        var dir = pos - centerPoint;
                        StartDirection = Vector2.Transform(dir, Matrix.CreateRotationZ(MathLib.LinearInterpolate(SpawnNoiseAngle.X, SpawnNoiseAngle.Y, random.NextDouble())));
                        StartDirection.Normalize();
                    }

                    Vector2 EndDirection = StartDirection * MathLib.LinearInterpolate(EndSpeed.X, EndSpeed.Y, random.NextDouble());
                    StartDirection *= MathLib.LinearInterpolate(StartSpeed.X, StartSpeed.Y, random.NextDouble());

                    var particle = new Particle(pos,
                        StartDirection,
                        EndDirection,
                        MathLib.LinearInterpolate(StartLife.X, StartLife.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(StartScale.X, StartScale.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(EndScale.X, EndScale.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(StartColor1, StartColor2, random.NextDouble()),
                        MathLib.LinearInterpolate(EndColor1, EndColor2, random.NextDouble()),
                        ParticleSprite, Engine.TextureCache, MaxHeight, MaxWidth);

                    ActiveParticles.AddLast(particle);
                    Engine.LevelManager.RenderManager.RegisterRenderable(particle);
                    
                }
                MSecPassed -= NextSpawnIn;
                NextSpawnIn = MathLib.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
            }

            LinkedListNode<Particle> node = ActiveParticles.First;
            while (node != null)
            {
                bool isAlive = node.Value.Update(time, world);
                node = node.Next;
                if (!isAlive)
                {
                    if (node == null)
                    {
                        ActiveParticles.RemoveLast();

                        if (ActiveParticles.Count != 0)
                        {
                            Engine.LevelManager.RenderManager.UnregisterRenderable(ActiveParticles.Last.Value);
                        }
                    }
                    else
                    {
                      ActiveParticles.Remove(node.Previous);
                      Engine.LevelManager.RenderManager.UnregisterRenderable(node.Value);
                      if (node.Previous != null)
                      {
                          Engine.LevelManager.RenderManager.UnregisterRenderable(node.Previous.Value);
                      }
                      

                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            LinkedListNode<Particle> node = ActiveParticles.First;
            while (node != null)
            {
         //       node.Value.Draw(spriteBatch, Scale, Offset);
           //     node = node.Next;
            }
        }

        public void Clear()
        {

            foreach (Particle p in ActiveParticles)
            {
                Engine.LevelManager.RenderManager.UnregisterRenderable(p);
            }
            ActiveParticles.Clear();
        }


        public override string EditorName
        {
            get { throw new NotImplementedException(); }
        }

        public override Guid Id
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override Engine.Core.Rendering.IRendering Preview
        {
            get { throw new NotImplementedException(); }
        }

        public override List<FarseerPhysics.Dynamics.Fixture> CollisionGeometry
        {
            get { throw new NotImplementedException(); }
        }

        public override List<Engine.Core.Rendering.IRendering> Renderings
        {
            get { return new List<IRendering> { new NullRendering()}; }
        }
    }
}
