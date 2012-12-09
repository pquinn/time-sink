using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using Autofac;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core.Editor;
using FarseerPhysics.Dynamics.Contacts;
using System.IO;

namespace TimeSink.Entities.Objects
{
    public enum DoorType { Up, Down, Side }

    [SerializableEntity("66c116cc-60bf-4808-a4c0-f5bb8cad053b")]
    [EditorEnabled]
    public class Door : Entity
    {
        const string EDITOR_NAME = "Door";
        const string TEXTURE = "Materials/blank";
        const string EDITOR_PREVIEW = "Textures/Objects/ladder";

        private static readonly Guid guid = new Guid("66c116cc-60bf-4808-a4c0-f5bb8cad053b");

        private bool collided;
        private EngineGame engine;

        public Door()
            : this(Vector2.Zero, 50, 50, DoorType.Up, string.Empty)
        {
        }

        public Door(Vector2 position, int width, int height, DoorType doorType, string levelPath)
        {
            Position = position;
            Width = width;
            Height = height;
            DoorType = doorType;
            LevelPath = levelPath;
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }
        
        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("DoorType")]
        public DoorType DoorType { get; set; }

        [SerializableField]
        [EditableField("LevelPath")]
        public string LevelPath { get; set; }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            if (DoorType == DoorType.Side)
                ChangeLevel();

            collided = true;

            return true;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            collided = false;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<World>();
                engine = engineRegistrations.Resolve<EngineGame>();
                Physics = BodyFactory.CreateBody(world, Position, this);

                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                var rect = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters,
                    1.4f,
                    Vector2.Zero,
                    Physics);

                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = true;

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);

                initialized = true;
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            Physics.Friction = 0;
            if (collided)
            {
                if ((InputManager.Instance.IsNewKey(Keys.W) && DoorType == DoorType.Up) ||
                    (InputManager.Instance.IsNewKey(Keys.S) && DoorType != DoorType.Up))
                {
                    ChangeLevel();
                }
            }
        }

        private void ChangeLevel()
        {
            engine.MarkAsLoadLevel(LevelPath);
        }

        public override void Load(IComponentContext engineRegistrations)
        {
        }

        public override IRendering Preview
        {
            get 
            { 
                return new SizedRendering(
                    EDITOR_PREVIEW, 
                    PhysicsConstants.MetersToPixels(Physics.Position), 
                    0, Width, Height); 
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override IRendering Rendering
        {
            get { return new NullRendering(); }
        }
    }
}
