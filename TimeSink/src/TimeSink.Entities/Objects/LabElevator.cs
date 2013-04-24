using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Enemies;
using TimeSink.Entities.Utils;

namespace TimeSink.Entities.Objects
{
    [EditorEnabled]
    [SerializableEntity("c31fb7ad-f3de-4ca3-a091-521583c6c6bf")]
    public class LabElevator : Entity, ISwitchable
    {
        const string TEXTURE = "Textures/Objects/Lab_Elevator";
        const string EDITOR_NAME = "Lab Elevator";

        private static readonly Guid GUID = new Guid("c31fb7ad-f3de-4ca3-a091-521583c6c6bf");
        private int segment = -1;
        private bool done;

        public LabElevator()
            : this(Vector2.Zero, Vector2.Zero, Vector2.Zero, false, Vector2.Zero, 0, 0, 0)
        {
        }

        public LabElevator(Vector2 startPosition, Vector2 endPosition2, Vector2 endPosition3,
            bool enabled, Vector2 endPosition, float timeSpan, int width, int height)
            : base()
        {
            Position = startPosition;
            StartPosition = startPosition;
            EndPosition = endPosition;
            EndPosition2 = endPosition2;
            EndPosition3 = endPosition3;
            TimeSpan = timeSpan;
            Width = width > 0 ? width : 50;
            Height = height > 0 ? width : 50;
            Enabled = enabled;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>()
                {
                    new BasicRendering(TEXTURE)
                    { 
                        Position = PhysicsConstants.MetersToPixels(Position),// - new Vector2(Width / 2, Height / 2),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache),
                       // TintColor = Color.Gray
                    }
                };
            }
        }

        [SerializableField]
        [EditableField("Start Position")]
        public Vector2 StartPosition { get; set; }

        [SerializableField]
        [EditableField("End Position")]
        public Vector2 EndPosition { get; set; }

        [SerializableField]
        [EditableField("End Position 2")]
        public Vector2 EndPosition2 { get; set; }

        [SerializableField]
        [EditableField("End Position 3")]
        public Vector2 EndPosition3 { get; set; }

        [SerializableField]
        [EditableField("Time Span")]
        public float TimeSpan { get; set; }

        [SerializableField]
        [EditableField("Enabled")]
        public bool Enabled { get; set; }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (InputManager.Instance.ActionPressed(InputManager.ButtonActions.AimDown))
            {
                OnSwitch();
            }

            if (!done && Vector2.Distance(Position, EndPosition) < .1)
                OnSwitch();
        }

        public void OnSwitch()
        {
            Physics.LinearVelocity = Vector2.Zero;

            segment++;
            if (segment >= 3)
            {
                CreateWave2();
                Enabled = false;
                done = true;
            }
            else
            {
                switch (segment)
                {
                    case 0:
                        SendOff();
                        break;
                    case 1:
                        CreateWave0();
                        StartPosition = Position;
                        EndPosition = EndPosition2;
                        break;
                    case 2:
                        CreateWave1();
                        StartPosition = Position;
                        EndPosition = EndPosition3;
                        break;
                }
            }
        }

        private void CreateWave0()
        {
            CreateWave(2);
        }

        private void Wave0Dead()
        {
            switch (segment)
            {
                case 1:
                    SendOff();
                    break;
                case 2:
                    SendOff();
                    break;
            }
            //var hopper1 = new Hopper(Position + new Vector2(xOff * rand.Next(0, 80) / 100, yOff));
            //var hopper2 = new Hopper(Position + new Vector2(-xOff * rand.Next(0, 80) / 100, yOff));
        }

        private void CreateWave1()
        {
            CreateWave(4);
        }

        private void CreateWave2()
        {
            CreateWave(6);
        }

        private void CreateWave(int numHoppers)
        {
            var xOff = PhysicsConstants.PixelsToMeters(Width / 2);
            var yOff = PhysicsConstants.PixelsToMeters(-Engine.GraphicsDevice.Viewport.Height);
            var rand = new Random();
            var list = new List<Enemy>();

            for (int i = 0; i < numHoppers; i++)
            {
                list.Add(new Hopper(Position + new Vector2(xOff * rand.Next(-40, 40) / 100, yOff)));
            }

            var wave = new Wave(list);
            wave.BatchCount = 2;
            wave.WaveDead += Wave0Dead;
            wave.Init(Engine);    
        }

        private void SendOff()
        {
            var off = EndPosition - StartPosition;
            var dir = Vector2.Normalize(off);

            Physics.LinearVelocity = dir * (off.Length() / TimeSpan);
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var halfHeight = PhysicsConstants.PixelsToMeters(Height / 2);
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);// + new Vector2(0, halfHeight));
                Physics.UserData = this;
                Physics.BodyType = BodyType.Kinematic;
                Physics.Friction = .25f;
                Physics.IgnoreGravity = true;
                Physics.CollidesWith = ~Category.Cat1;
                Physics.CollisionCategories = Category.Cat1;

                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat1;
                fix.CollidesWith = ~Category.Cat1;

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override IRendering Preview
        {
            get { return Renderings[0]; }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }
    }
}
