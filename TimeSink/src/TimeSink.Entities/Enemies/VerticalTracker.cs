using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Autofac;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Factories;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("a0a2ba1a-0692-4f49-adb5-1f333e462649")]
    public class VerticalTracker : Enemy
    {
        private const string TEXTURE = "";
        private float DEPTH = -50f;

        [SerializableField]
        [EditableField("Trigger")]
        public string Trigger { get; set; }

        private UserControlledCharacter target;

        private static readonly Guid guid = new Guid("a0a2ba1a-0692-4f49-adb5-1f333e462649");

        public VerticalTracker()
            : this(Vector2.Zero, "")
        {
        }


        public VerticalTracker(Vector2 position, string trigger)
            :base(position)
        {
            Trigger = trigger;
        }

        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);
            var width = PhysicsConstants.PixelsToMeters(Width);
            var height = PhysicsConstants.PixelsToMeters(Height);
            var world = engineRegistrations.Resolve<PhysicsManager>().World;

            FixtureFactory.AttachRectangle(width, height, .1f, Vector2.Zero, Physics);

            
                    
        }

        public void Jump()
        {
            Physics.ApplyForce(new Vector2(0, -5));
        }

        public void Shoot()
        {
        }
    }
}
