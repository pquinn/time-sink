using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Engine.Defaults;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Entities.Triggers
{
    [SerializableEntity("75522a0f-56c2-444e-90bb-88df79c36c29")]
    [EditorEnabled]
    public class SlideTrigger : Trigger
    {
        const string EDITOR_NAME = "Slide Trigger";
        private static readonly Guid GUID = new Guid("75522a0f-56c2-444e-90bb-88df79c36c29");
        
        public SlideTrigger() : base() { }

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

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            c.AddSlideTrigger(this);

            return true;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            c.RemoveSlideTrigger(this);
        }

        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
            Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);
        }
    }
}
