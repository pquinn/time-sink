using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Game.Entities;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace TimeSink.Entities
{
    public delegate void TriggerDelegate(ICollideable collided);

    public class Trigger : Entity
    {
        public Body PhysicsBody { get; private set; }

        const string EDITOR_NAME = "Trigger";

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

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Rendering
        {
            get { return null; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {

        }

        public override void Load(EngineGame engineGame)
        {

        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(ICollideable obj, Contact info)
        {
            if (Triggered != null)
                Triggered(obj);
        }

        public override void InitializePhysics(World world)
        {
            PhysicsBody = BodyFactory.CreateBody(world, _position, this);
            PhysicsBody.BodyType = BodyType.Static;
            PhysicsBody.IsSensor = true;
            _geom = PhysicsBody.FixtureList;
        }
    }
}