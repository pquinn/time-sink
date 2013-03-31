using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Particles;

namespace TimeSink.Entities.Objects
{

    public class Emitter : Entity
    {
        public Vector2 RelPosition;                    // Position relative to collection.
        public int Budget;                      // Max number of alive particles.
        float NextSpawnIn;                      // This is a random number generated using the SecPerSpawn.
        float SecPassed;                        // Time pased since last spawn.
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

        public Emitter(Vector2 SecPerSpawn, Vector2 SpawnDirection, Vector2 SpawnNoiseAngle, Vector2 StartLife, Vector2 StartScale,
                    Vector2 EndScale, Color StartColor1, Color StartColor2, Color EndColor1, Color EndColor2, Vector2 StartSpeed,
                    Vector2 EndSpeed, int Budget, Vector2 RelPosition, string ParticleSprite, Random random)
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
            this.SecPassed = 0.0f;
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            SecPassed += time.ElapsedGameTime.Seconds;
            while (SecPassed > NextSpawnIn)
            {
                if (ActiveParticles.Count < Budget)
                {
                    // Spawn a particle
                    Vector2 StartDirection = Vector2.Transform(SpawnDirection, Matrix.CreateRotationZ(MathLib.LinearInterpolate(SpawnNoiseAngle.X, SpawnNoiseAngle.Y, random.NextDouble())));
                    StartDirection.Normalize();
                    Vector2 EndDirection = StartDirection * MathLib.LinearInterpolate(EndSpeed.X, EndSpeed.Y, random.NextDouble());
                    StartDirection *= MathLib.LinearInterpolate(StartSpeed.X, StartSpeed.Y, random.NextDouble());
                    ActiveParticles.AddLast(new Particle(
                        RelPosition,
                        StartDirection,
                        EndDirection,
                        MathLib.LinearInterpolate(StartLife.X, StartLife.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(StartScale.X, StartScale.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(EndScale.X, EndScale.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(StartColor1, StartColor2, random.NextDouble()),
                        MathLib.LinearInterpolate(EndColor1, EndColor2, random.NextDouble()),
                        this, ParticleSprite, Engine.TextureCache)
                    );
                }
                SecPassed -= NextSpawnIn;
                NextSpawnIn = MathLib.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
            }

            LinkedListNode<Particle> node = ActiveParticles.First;
            while (node != null)
            {
                bool isAlive = node.Value.Update(time);
                node = node.Next;
                if (!isAlive)
                {
                    if (node == null)
                    {
                        ActiveParticles.RemoveLast();
                    }
                    else
                    {
                        ActiveParticles.Remove(node.Previous);
                    }
                }
            }
        }


        public void Clear()
        {
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
            get { throw new NotImplementedException(); }
        }
    }
}
