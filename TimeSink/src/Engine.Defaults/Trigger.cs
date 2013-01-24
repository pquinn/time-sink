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

namespace Engine.Defaults
{
    public delegate void TriggerDelegate(Entity collided);

    [SerializableEntity("f3722310-9db5-478f-9e37-608cbcbf92f9")]
    public class Trigger : Entity
    {
        const string EDITOR_NAME = "Trigger";

        private static readonly Guid GUID = new Guid("f3722310-9db5-478f-9e37-608cbcbf92f9");

        public event TriggerDelegate Triggered;

        private List<Fixture> _geom;
        public override List<Fixture> CollisionGeometry
        {
            get { return _geom; }
        }

        public Trigger()
        {

        }

        public Trigger(Vector2 position)
        {
            Position = position;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }
        
        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override IRendering Rendering
        {
            get { return new NullRendering(); }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {

        }

        public override void Load(IComponentContext engineRegistrations)
        {

        }

        public virtual bool OnCollidedWith(Fixture f, Entity obj, Fixture f2, Contact info)
        {
            return true;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;

                Physics = BodyFactory.CreateBody(world, Position, this);
                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = true;
                _geom = Physics.FixtureList;

                Physics.RegisterOnCollidedListener<Entity>(OnCollidedWith);

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }
    }
}