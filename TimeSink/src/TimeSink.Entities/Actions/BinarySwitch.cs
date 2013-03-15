using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Utils;

namespace TimeSink.Entities.Actions
{
    [SerializableEntity("fe4f39bb-fb1e-4ce0-be2e-92ca70975f4e")]
    [EditorEnabled]
    class BinarySwitch : InteractableItem
    {
        private string editorName = "Binary Switch";
        const string TEXTURE = "Textures/HUD/HealthBarTemp";
        private static readonly Guid guid = new Guid("fe4f39bb-fb1e-4ce0-be2e-92ca70975f4e");


        public BinarySwitch()
            : this(false, null)
        { }


        public BinarySwitch(bool enabled, string target)
        {
            Enabled = enabled;
            Target = target;
        }


        [SerializableField]
        [EditableField("Target")]
        public string Target { get; set; }


        [SerializableField]
        [EditableField("Enabled")]
        public bool Enabled { get; set; }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache),
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
            Target.OnSwitch();

            Enabled = !Enabled;

            SwitchEnabledState();
        }

        //For switching current animation/display of switch.
        private void SwitchEnabledState()
        {
        }

    }
}
