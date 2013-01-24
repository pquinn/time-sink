using Autofac;
using Engine.Defaults;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Entities.Enemies;

namespace TimeSink.Entities.Triggers
{
    [EditorEnabled]
    [SerializableEntity("2605fa3e-389a-4ee4-a0d0-d576ab189404")]
    public class TutorialMonsterCreateTrigger : Entity
    {
        const string EDITOR_NAME = "Tutorial Monster Create Trigger";

        private static readonly Guid GUID = new Guid("2605fa3e-389a-4ee4-a0d0-d576ab189404");

        private LevelManager levelManager;

        private List<Fixture> _geom;
        public override List<Fixture> CollisionGeometry
        {
            get { return _geom; }
        }

        public TutorialMonsterCreateTrigger()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public TutorialMonsterCreateTrigger(Vector2 position, int width, int height)
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

        private bool used;
        public virtual bool OnCollidedWith(Fixture f, UserControlledCharacter obj, Fixture f2, Contact info)
        {
            if (!used)
            {
                var monster = new TutorialMonster(Position - new Vector2(0, PhysicsConstants.PixelsToMeters(800)), Vector2.UnitX);
                levelManager.RegisterEntity(monster);

                used = true;
            }

            return true;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                levelManager = engineRegistrations.Resolve<LevelManager>();
                Physics = BodyFactory.CreateRectangle(world, PhysicsConstants.PixelsToMeters(Width), PhysicsConstants.PixelsToMeters(Height), 1, Position);
                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = true;
                _geom = Physics.FixtureList;

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

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
