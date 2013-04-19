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
using TimeSink.Entities.Objects;


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
        const string TEXTURE = "Textures/Objects/FanAnim";

        private Vector2 localOriginPoint;
        private float forceFactor = 150;
        private double nextFlipTime = 0;
        private bool collided;
        private UserControlledCharacter _character;
        private float _frictionHolder;
        private Emitter particles;
        private float animInterval = 50f;
        private float animTimer = 0f;
        private NewAnimationRendering rendering;
        private static readonly Vector2 SrcRectSize = new Vector2(158.9999f, 97);
        private float _torqueHolder;

        public FanTrigger()
            : base()
        {
            rendering = new NewAnimationRendering(
                TEXTURE,
                SrcRectSize,
                24,
                PhysicsConstants.MetersToPixels(position),
                0,
                new Vector2(.75f, .35f),
                Color.White) { DepthWithinLayer = 100 };
        }

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

        public bool IsSideways { 
            get { 
                return FanDirection == FanDirection.Left || FanDirection == FanDirection.Right; 
            } 
        }

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
                forceFactor = (float)(Math.Sqrt(2 * Engine.LevelManager.PhysicsManager.World.Gravity.Y) * c.Physics.Mass) / (4.5f * Length());

                if (!IsSideways)
                {
                    
                }
                else
                {
                    forceFactor *= 3;
                    _frictionHolder = c.WheelBody.Friction;
                    c.WheelBody.Friction = .01f;
                    _torqueHolder = c.MotorJoint.MotorTorque;
                    c.MotorJoint.MotorTorque = .01f;
                }

                _character = c;
                collided = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private Vector2 CalculateForce(Vector2 characterPosition, float elapsedTime)
        {
            float originDirection, characterDirection, dimensionDirection;
            var originPointInMeters = PhysicsConstants.PixelsToMeters(localOriginPoint);
            var originPointInWorld = Position + originPointInMeters;

            if (!IsSideways)
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

            //var magnitude = (dimensionDirection - (Math.Abs((characterDirection - originDirection)))) / dimensionDirection;
            var amount = Length() - Math.Abs(characterDirection - originDirection) / Length();
            var magnitude = amount * forceFactor * elapsedTime;
            return DetermineDirection(magnitude);
        }

        private float Length()
        {
            if (FanDirection == FanDirection.Up || FanDirection == FanDirection.Down)
                return PhysicsConstants.PixelsToMeters(Height);
            else
                return PhysicsConstants.PixelsToMeters(Width);
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
            if (IsSideways)
            {
                c.Physics.Friction = _frictionHolder;
                c.MotorJoint.MotorTorque = _torqueHolder;
            }
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
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>()
                {  
                    rendering
                };
            }
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

            animTimer += (float)time.ElapsedGameTime.Milliseconds;

            switch (FanDirection)
            {
                case FanDirection.Up:
                    rendering.Position = PhysicsConstants.MetersToPixels(position) + new Vector2(0,(Height / 2));
                    break;
                case FanDirection.Down:
                    rendering.Position = PhysicsConstants.MetersToPixels(position) + new Vector2(0,-(Height / 2));
                    rendering.Rotation = (float)Math.PI;
                    break;
                case FanDirection.Left:
                    rendering.Position = PhysicsConstants.MetersToPixels(position) + new Vector2((Width / 2), 0);
                    rendering.Rotation = (float)- Math.PI / 2;
                    break;
                case FanDirection.Right:
                    rendering.Position = PhysicsConstants.MetersToPixels(position) + new Vector2(-(Width / 2), 0);
                    rendering.Rotation = (float)Math.PI / 2;
                    break;
            }
            if (Active)
            {
                if (animTimer >= animInterval)
                {
                    rendering.CurrentFrame = (rendering.CurrentFrame + 1) % rendering.NumFrames;
                    animTimer = 0f;
                }
            }
            if (time.TotalGameTime.TotalMilliseconds >= nextFlipTime)
            {
                if (Active)
                {
                    nextFlipTime = time.TotalGameTime.TotalMilliseconds + InactiveTime;

                    particles.Clear();
                    Engine.LevelManager.UnregisterEntity(particles);
                    particles = null;
                }
                else
                {
                    nextFlipTime = time.TotalGameTime.TotalMilliseconds + IntervalDuration;
                }
                Active = !Active;

            }

            if (Active && particles == null)
            {
                Vector2 offset = Vector2.Zero;
                switch(FanDirection)
                {
                    case FanDirection.Up:
                        offset = new Vector2(0,  PhysicsConstants.PixelsToMeters(Height) / 2);
                        break;
                    case FanDirection.Down:
                        offset = new Vector2(0, -PhysicsConstants.PixelsToMeters(Height) / 2);
                        break;
                    case FanDirection.Left:
                        offset = new Vector2(PhysicsConstants.PixelsToMeters(Width) / 2, 0);
                        break;
                    case FanDirection.Right:
                        offset = new Vector2(-PhysicsConstants.PixelsToMeters(Width) / 2, 0);
                        break;
                }
                particles = new Emitter(new Vector2(100f, 100f),
                    DetermineDirection(1.0f), new Vector2(-.1f, .1f), new Vector2(4000f, 4000f),
                    Vector2.One, Vector2.One, Color.White, Color.Red, Color.White, Color.Red,
                    new Vector2(0, PhysicsConstants.PixelsToMeters(1f)), new Vector2(0, PhysicsConstants.PixelsToMeters(1f)), 100, Vector2.Zero, "Textures/Objects/dust", new Random(), Position + offset,
                    PhysicsConstants.PixelsToMeters(Width * 2), PhysicsConstants.PixelsToMeters(Height * 2));
                Engine.LevelManager.RegisterEntity(particles);
            }

            if (Enabled && collided && Active)
            {
                var force = CalculateForce(_character.Position, time.ElapsedGameTime.Milliseconds);
                _character.Physics.ApplyForce(force);
                _character.WheelBody.ApplyForce(force);
            }

            base.OnUpdate(time, world);
        }
    }
}
