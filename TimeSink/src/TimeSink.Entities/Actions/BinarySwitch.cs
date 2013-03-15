using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Objects;
using TimeSink.Entities.Utils;

namespace TimeSink.Entities.Actions
{
    [SerializableEntity("fe4f39bb-fb1e-4ce0-be2e-92ca70975f4e")]
    [EditorEnabled]
    class BinarySwitch : InteractableItem
    {
        const string EDITOR_NAME = "Binary Switch";
        const string ENABLED_TEXTURE = "Textures/HUD/HealthBarTemp";
        const string DISABLED_TEXTURE = "Textures/giroux";
        private static readonly Guid guid = new Guid("fe4f39bb-fb1e-4ce0-be2e-92ca70975f4e");


        public BinarySwitch()
            : this(false, String.Empty)
        { }


        public BinarySwitch(bool enabled, string targets)
        {
            Enabled = enabled;
            Targets = targets;
        }


        [SerializableField]
        [EditableField("Targets")]
        public string Targets { get; set; }

        public List<String> TargetsList
        {
            get
            {
                return Targets.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        private List<ISwitchable> targetObjects = new List<ISwitchable>();

        [SerializableField]
        [EditableField("Enabled")]
        public bool Enabled { get; set; }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get
            {
                var currentTexture = Enabled ? ENABLED_TEXTURE : DISABLED_TEXTURE;
                return new BasicRendering(currentTexture)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Scale = BasicRendering.CreateScaleFromSize(Width, Height, currentTexture, TextureCache),
                    DepthWithinLayer = 0
                };
            }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() { Preview };
            }
        }

        protected override void ExecuteAction()
        {
            foreach (ISwitchable target in targetObjects)
            {
                target.OnSwitch();
            }

            SwitchEnabledState();
        }

        private void SwitchEnabledState()
        {
            Enabled = !Enabled;
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);

            foreach (string key in TargetsList)
            {
                var target = engine.LevelManager.Level.Entities.First(x => x.InstanceId.Equals(key)) as ISwitchable;
                if (target != null) targetObjects.Add(target);
            }
        }

    }
}
