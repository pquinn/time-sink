using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Weapons;

namespace TimeSink.Entities.Objects
{

    [EditorEnabled]
    [SerializableEntity("eaa35946-ea93-4e3b-968c-7e6d0c6dbb34")]
    class BowPickup : InteractableItem
    {
        const  string EDITOR_NAME = "BowPickup";
        const  string TEXTURE = "Textures/Weapons/Bow_Neutral";

        private static readonly Guid guid = new Guid("eaa35946-ea93-4e3b-968c-7e6d0c6dbb34");

        public BowPickup()
            : base()
        {
            
        }


        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        public override Engine.Core.Rendering.IRendering Preview
        {
            get
            {
                return new SizedRendering(
                    TEXTURE,
                    PhysicsConstants.MetersToPixels(Position),
                    0, Width, Height);
            }
        }
        public override Engine.Core.Rendering.IRendering Rendering
        {
            get
            {
                return Preview;
            }
        }

        protected override void ExecuteAction()
        {
            base.ExecuteAction();

            DestroyPhysics();

            Character.AddInventoryItem(new Arrow());

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
