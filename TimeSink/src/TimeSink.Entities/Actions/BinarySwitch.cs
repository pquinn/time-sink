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
using TimeSink.Engine.Core;

namespace TimeSink.Entities.Actions
{
    [SerializableEntity("fe4f39bb-fb1e-4ce0-be2e-92ca70975f4e")]
    [EditorEnabled]
    class BinarySwitch : InteractableItem
    {
        const string EDITOR_NAME = "Binary Switch";
        const string ENABLED_TEXTURE = "Textures/Objects/switchOn";
        const string DISABLED_TEXTURE = "Textures/Objects/switchOff";
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
        [EditableField("Is Toggler")]
        public bool IsToggler { get; set; }

        [SerializableField]
        [EditableField("Targets")]
        public string Targets { get; set; }

        [SerializableField]
        [EditableField("Enabled")]
        public bool Enabled { get; set; }

        public List<String> TargetsList
        {
            get
            {
                return Targets.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        private List<ISwitchable> targetObjects = new List<ISwitchable>();

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
            SwitchEnabledState();

            foreach (ISwitchable target in targetObjects)
            {
                if (IsToggler)
                    target.Enabled = !target.Enabled;
                else
                    target.Enabled = Enabled;
                target.OnSwitch();
            }
        }

        private void SwitchEnabledState()
        {
            Enabled = !Enabled;
            engine.LevelManager.LevelCache.ReplaceOrAdd(InstanceId, Enabled);
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);

            Physics.IsSensor = true;

            object cachedState = null;
            if (engine != null && engine.LevelManager.LevelCache.TryGetValue(InstanceId, out cachedState))
            {
                Enabled = (bool)cachedState;
            }

            foreach (string key in TargetsList)
            {
                var target = 
                    engine == null ? null : engine.LevelManager.Level.Entities.First(x => x.InstanceId.Equals(key)) as ISwitchable;
                if (target != null)
                {
                    targetObjects.Add(target);
                    if (!IsToggler)
                        target.Enabled = Enabled;
                }
            }
        }

    }
}
