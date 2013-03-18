using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace TimeSink.Entities.Enemies
{
    [SerializableEntity("5774325e-ce5e-4db6-a036-4ed8e85a36d4")]
    [EditorEnabled]
    public class Turret : Entity
    {
        const string EDITOR_NAME = "Turret";
        private static readonly Guid GUID = new Guid("5774325e-ce5e-4db6-a036-4ed8e85a36d4");
        private static readonly string BASE_ON = "Textures/Objects/MG-Turret-Base_Off";
        private static readonly string BASE_OFF = "Textures/Objects/MG-Turret-Base_Off";
        private static readonly string GUN = "Textures/Objects/MG-Turret-gun";

        private float gunRotation = 0f;
        private Guid charGuid = new Guid("defb4f64-1021-420d-8069-e24acebf70bb");

        public Turret() : base() { }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public override Guid Id
        {
            get { return GUID; }
            set { }
        }

        public bool Enabled { get; set; }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            var offset = world.LevelManager.Level.Entities.First(x => x.Id == charGuid).Position - Position;
            gunRotation = (float)Math.Atan2(offset.Y, offset.X);
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>()
                {
                    new BasicRendering(Enabled ? BASE_ON : BASE_OFF)
                    {
                        Position = Position
                    },
                    new BasicRendering(GUN)
                    {
                        Position = Position,
                        Rotation = gunRotation
                    }
                };
            }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(Enabled ? BASE_ON : BASE_OFF)
                {
                    Position = Position
                };
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }
    }
}
