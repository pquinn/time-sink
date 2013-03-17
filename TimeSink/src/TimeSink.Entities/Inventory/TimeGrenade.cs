using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using Autofac;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities.Inventory
{
    public class TimeGrenade : Entity
    {
        const string EDITOR_NAME = "Time Grenade";

        private Vector2 initialVelocity;

        [SerializableEntity("16b7d25a-15f1-4b0b-acaf-c70124acda0e")]
        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        private static readonly Guid GUID = new Guid("16b7d25a-15f1-4b0b-acaf-c70124acda0e");
        public override Guid Id { get { return GUID; } set { } }

        private double fuseTime;

        public override IRendering Preview
        {
            get { return new NullRendering(); }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override List<IRendering> Renderings
        {
            get { return new List<IRendering>(); }
        }

        public TimeGrenade(Vector2 pos, Vector2 vel, double fuseTime_ms, double lingerTime_ms)
        {
            Position = pos;
            initialVelocity = vel;
            fuseTime = fuseTime_ms;
            lingerTime = lingerTime_ms;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                Physics = BodyFactory.CreateCircle(
                    world,
                    PhysicsConstants.PixelsToMeters(7),
                    1,
                    Position,
                    this);

                Physics.BodyType = BodyType.Dynamic;
                Physics.CollidesWith = Category.All;
                Physics.LinearVelocity = initialVelocity;
            }
        }

        private bool exploded;
        private double lingerTime;

        private TimeScaleCircle blast;

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (!exploded)
            {
                fuseTime -= time.ElapsedGameTime.TotalMilliseconds;
                if (fuseTime <= 0)
                {
                    exploded = true;
                    blast = new TimeScaleCircle()
                    {
                        Center = Position,
                        Radius = PhysicsConstants.PixelsToMeters(100),
                        TimeScale = .3f
                    };
                    world.LevelManager.Level.TimeScaleCircles.Add(blast);

                    //TODO: Stop drawing sprite
                }
            }
            else
            {
                lingerTime -= time.ElapsedGameTime.TotalMilliseconds;
                if (lingerTime <= 0)
                {
                    world.LevelManager.RenderManager.UnregisterRenderable(this);
                    world.LevelManager.PhysicsManager.UnregisterPhysicsBody(this);
                    world.LevelManager.Level.TimeScaleCircles.Remove(blast);
                    Dead = true;
                }
            }
        }
    }
}
