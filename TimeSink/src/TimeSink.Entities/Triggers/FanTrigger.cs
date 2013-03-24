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
using TimeSink.Entities.Utils;

namespace TimeSink.Entities.Triggers
{
    [SerializableEntity("89bb3358-5b34-43ca-8bd3-fa21322159ba")]
    [EditorEnabled]
    public class FanTrigger : Trigger, ISwitchable
    {
        const string EDITOR_NAME = "Fan Trigger";
        private static readonly Guid GUID = new Guid("89bb3358-5b34-43ca-8bd3-fa21322159ba");

        private Vector2 originPoint;
        private int forceFactor = 1100;

        private double nextLogTime = 0;
        
        public FanTrigger() : base() { }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        [EditableField("Interval (ms)")]
        public int IntervalDuration { get; set; }

        [SerializableField]
        public override Guid Id
        {
            get { return GUID; }
            set { }
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            if (cf.UserData.Equals("Ladder") && Active)
            {
                c.Physics.IgnoreGravity = true;
                c.CanJump = false;
                c.Physics.LinearVelocity = Vector2.Zero;
                var originPointInMeters = PhysicsConstants.PixelsToMeters(originPoint);
                var originPointInWorld = Position + originPointInMeters;
                var magnitude = (100 - ((originPointInWorld.Y - c.Position.Y) / PhysicsConstants.PixelsToMeters(Height))) / 100;
                c.Physics.ApplyForce(new Vector2(0, -forceFactor * magnitude));
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            c.CanJump = true;
            c.Physics.IgnoreGravity = false;
        }

        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
            Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);
        }

        public bool Enabled { get; set; }

        public bool Active { get; set; }

        public void OnSwitch()
        {
            Enabled = !Enabled;
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);
            originPoint = new Vector2(0, Height / 2);
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (time.TotalGameTime.TotalMilliseconds >= nextLogTime)
            {
                Active = !Active;   
                nextLogTime = time.TotalGameTime.TotalMilliseconds + IntervalDuration;
            }
            base.OnUpdate(time, world);
        }
    }
}
