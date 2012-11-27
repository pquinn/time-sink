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

namespace TimeSink.Entities
{
    public delegate void TriggerDelegate(ICollideable collided);

    [SerializableEntity("f3722310-9db5-478f-9e37-608cbcbf92f9")]
    public class Trigger : Entity
    {
        const string EDITOR_NAME = "Trigger";

        private static readonly Guid GUID = new Guid("f3722310-9db5-478f-9e37-608cbcbf92f9");

        private Vector2 _position;

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
            _position = position;
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
            get { return null; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {

        }

        public override void Load(IComponentContext engineRegistrations)
        {

        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(ICollideable obj, Contact info)
        {
            if (Triggered != null)
                Triggered(obj);
        }

        public override void InitializePhysics(IComponentContext engineRegistrations)
        {
            var world = engineRegistrations.Resolve<World>();

            Physics = BodyFactory.CreateBody(world, _position, this);
            Physics.BodyType = BodyType.Static;
            Physics.IsSensor = true;
            _geom = Physics.FixtureList;
        }
    }
}