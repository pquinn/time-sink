using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Autofac;
using System.Xml.Serialization;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.StateManagement;

namespace Engine.Defaults
{
    public delegate void TriggerDelegate(Entity collided);

    public abstract class Trigger : Entity
    {
        protected LevelManager levelManager;
        protected EngineGame engineGame;

        protected const float DEPTH = 0;
        const string TEXTURE = "Textures/trigger";

        protected List<Fixture> _geom;
        public override List<Fixture> CollisionGeometry
        {
            get { return _geom; }
        }

        public Trigger()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public Trigger(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        public Action<Body> Registrations { get; set; }
        
        public override List<IRendering> Renderings
        {
            get { return new List<IRendering>() { new NullRendering()}; }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache),
                    DepthWithinLayer = DEPTH
                };
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(IComponentContext engineRegistrations)
        {
        }

        protected bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                levelManager = engineRegistrations.Resolve<LevelManager>();
                engineGame = engineRegistrations.ResolveOptional<EngineGame>();

                Physics = BodyFactory.CreateRectangle(
                    world, 
                    PhysicsConstants.PixelsToMeters(Width), 
                    PhysicsConstants.PixelsToMeters(Height), 
                    1, 
                    Position, 
                    this);
                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = true;
                _geom = Physics.FixtureList;

                RegisterCollisions();

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        protected abstract void RegisterCollisions();

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }
    }
}