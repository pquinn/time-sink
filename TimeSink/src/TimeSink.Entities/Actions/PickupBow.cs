using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.StateManagement;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Inventory;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities.Actons
{

    [EditorEnabled]
    [SerializableEntity("eaa35946-ea93-4e3b-968c-7e6d0c6dbb34")]
    class BowPickup : InteractableItem
    {
        const string EDITOR_NAME = "Pickup Bow";
        const  string TEXTURE = "Textures/Weapons/Bow_Neutral";
        const float DEPTH = 0;

        private static readonly Guid guid = new Guid("eaa35946-ea93-4e3b-968c-7e6d0c6dbb34");

        public BowPickup()
            : base()
        {
            
        }

        [SerializableField]
        [EditableField("Prompt")]
        public string Prompt { get; set; }

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
                    DepthWithinLayer = DEPTH
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
            base.ExecuteAction();

            DestroyPhysics();

            Character.AddInventoryItem(new Arrow());

            if (!engine.ScreenManager.IsInDialogueState())
                engine.ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid(Prompt)), null);
        }
        public override string EditorName
        {
            get
            {
               return EDITOR_NAME;
            }
        }
    }
}
