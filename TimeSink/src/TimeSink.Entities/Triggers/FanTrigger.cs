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


// make fans horizontal too
// separate interval from inactive time
namespace TimeSink.Entities.Triggers
{
    public enum FanDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializableEntity("89bb3358-5b34-43ca-8bd3-fa21322159ba")]
    [EditorEnabled]
    public class FanTrigger : Trigger, ISwitchable
    {
        const string EDITOR_NAME = "Fan Trigger";
        private static readonly Guid GUID = new Guid("89bb3358-5b34-43ca-8bd3-fa21322159ba");

        private Vector2 localOriginPoint;
        private int forceFactor = 50;
        private double nextFlipTime = 0;
        private bool collided;
        private UserControlledCharacter _character;
        
        public FanTrigger() : base() { }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        [EditableField("Active Interval (ms)")]
        public int IntervalDuration { get; set; }

        [SerializableField]
        [EditableField("Inactive Time (ms)")]
        public int InactiveTime { get; set; }

        [SerializableField]
        [EditableField("Fan Direction")]
        public FanDirection FanDirection { get; set; }

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
                _character = c;
                collided = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private Vector2 CalculateForce(Vector2 characterPosition)
        {
            float originDirection, characterDirection, dimensionDirection;
            var originPointInMeters = PhysicsConstants.PixelsToMeters(localOriginPoint);
            var originPointInWorld = Position + originPointInMeters;

            if (FanDirection == FanDirection.Up || FanDirection == FanDirection.Down)
            {
                originDirection = originPointInWorld.Y;
                characterDirection = characterPosition.Y;
                dimensionDirection = PhysicsConstants.PixelsToMeters(Height);
            }
            else
            {
                originDirection = originPointInWorld.X;
                characterDirection = characterPosition.X;
                dimensionDirection = PhysicsConstants.PixelsToMeters(Width);
            }

            var magnitude = (dimensionDirection - (Math.Abs((characterDirection - originDirection)))) / dimensionDirection;
            return DetermineDirection(magnitude * forceFactor);
        }

        private Vector2 DetermineDirection(float magnitude)
        {
            switch (FanDirection)
            {
                case FanDirection.Up:
                    return new Vector2(0, -1) * magnitude;
                case FanDirection.Down:
                    return new Vector2(0, 1) * magnitude;
                case FanDirection.Left:
                    return new Vector2(-1, 0) * magnitude;
                case FanDirection.Right:
                    return new Vector2(1, 0) * magnitude;
                default:
                    // this should never happen
                    return Vector2.Zero;
            }

            
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            c.CanJump = true;
            c.Physics.IgnoreGravity = false;
            collided = false;
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
            // divide measurement by 2 because origin is in center
            switch (FanDirection)
            {
                case FanDirection.Up:
                    localOriginPoint = new Vector2(0, Height / 2);
                    break;
                case FanDirection.Down:
                    localOriginPoint = new Vector2(0, -Height / 2);
                    break;
                case FanDirection.Left:
                    localOriginPoint = new Vector2(Width / 2, 0);
                    break;
                case FanDirection.Right:
                    localOriginPoint = new Vector2(-Width / 2, 0);
                    break;
                default:
                    localOriginPoint = new Vector2(0, 0);
                    break;
            }

            Enabled = true;
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (time.TotalGameTime.TotalMilliseconds >= nextFlipTime)
            {
                if (Active)
                {
                    nextFlipTime = time.TotalGameTime.TotalMilliseconds + InactiveTime;
                }
                else
                {
                    nextFlipTime = time.TotalGameTime.TotalMilliseconds + IntervalDuration;
                }
                Active = !Active;   
            }

            if (Enabled && collided && Active)
            {
                _character.Physics.ApplyForce(CalculateForce(_character.Position));
            }

            base.OnUpdate(time, world);
        }
    }
}
