using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Entities.Weapons;
using TimeSink.Engine.Core.Editor;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Common;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Objects;
using FarseerPhysics.Dynamics.Joints;

namespace TimeSink.Entities
{
    [SerializableEntity("defb4f64-1021-420d-8069-e24acebf70bb")]
    public class UserControlledCharacter : Entity, IHaveHealth, IHaveShield, IHaveMana
    {
        const float PLAYER_MASS = 100f;
        const string EDITOR_NAME = "User Controlled Character";

        private static readonly Guid GUID = new Guid("defb4f64-1021-420d-8069-e24acebf70bb");
        enum BodyStates
        {
            NeutralRight, NeutralLeft,
            IdleRightOpen, IdleRightClosed, IdleLeftOpen, IdleLeftClosed,
            WalkingStartRight, WalkingRight, WalkingEndRight, WalkingStartLeft, WalkingLeft, WalkingEndLeft,
            RunningIntermediateRight, RunningRight, RunningIntermediateLeft, RunningLeft,
            JumpingRight, JumpingLeft,
            ShootingRight, ShootingLeft,
            DuckingRight, DuckingLeft,
            ClimbingBack,
            ClimbingLeft, ClimbingRight
        };


        BodyStates currentState;

        //Texture strings for content loading
        const string JUMP_SOUND_NAME = "Audio/Sounds/Hop";

        const string EDITOR_PREVIEW = "Textures/Body_Neutral";

        const string NEUTRAL_RIGHT = "Textures/Sprites/SpriteSheets/Body_Neutral";
        const string NEUTRAL_LEFT = "Textures/Sprites/SpriteSheets/Neutral_Left";
        const string IDLE_CLOSED_HAND = "Textures/Sprites/SpriteSheets/Idle_OpenHand";
        const string IDLE_OPEN_HAND = "Textures/Sprites/SpriteSheets/Idle_OpenHand";
        const string WALKING_RIGHT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Walking_Right_Intermediate";
        const string WALKING_RIGHT = "Textures/Sprites/SpriteSheets/Body_Walking_Right";
        const string JUMPING_RIGHT = "Textures/Sprites/SpriteSheets/Jumping_Right";
        const string WALKING_LEFT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Walking_Intermediate_Left";
        const string WALKING_LEFT = "Textures/Sprites/SpriteSheets/BodyWalkLeft";
        const string JUMPING_LEFT = "Textures/Sprites/SpriteSheets/JumpingLeft";
        const string FACING_BACK = "Textures/Sprites/SpriteSheets/Backward";


        private Dictionary<BodyStates, NewAnimationRendering> animations;

        const float MAX_ARROW_HOLD = 1;
        const float MIN_ARROW_INIT_SPEED = 500;
        const float MAX_ARROW_INIT_SPEED = 1500;
        public static float X_OFFSET = PhysicsConstants.PixelsToMeters(60 - 65);
        public static float Y_OFFSET = PhysicsConstants.PixelsToMeters(80 - 141);

        private SoundEffect jumpSound;
        private bool jumpToggleGuard = true;
        private Rectangle sourceRect;
        private float health;
        private float mana;
        private float shield;
        private Ladder canClimb = null;
        private World _world;

        private Joint vineAttachment;

        public Ladder CanClimb { get { return canClimb; } set { canClimb = value; } }

        private List<IInventoryItem> inventory;
        private int activeItem;

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(
                    EDITOR_PREVIEW,
                    PhysicsConstants.MetersToPixels(Physics.Position),
                    playerRotation, Vector2.One);
            }
        }

        [SerializableField]
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        [SerializableField]
        public float Shield
        {
            get { return shield; }
            set { shield = value; }
        }

        [SerializableField]
        public float Mana
        {
            get { return mana; }
            set { mana = value; }
        }

        private float playerRotation = 0.0f;
        private int facing = 1; // 1 for right, -1 for left

        // not sure if these should be public
        private Vector2 direction;
        [SerializableField]
        public Vector2 Direction
        {
            get { return direction; }
            private set { direction = value; }
        }
        private double holdTime;
        [SerializableField]
        public double HoldTime
        {
            get { return holdTime; }
            set { holdTime = value; }
        }
        private bool inHold;
        [SerializableField]
        public bool InHold
        {
            get { return inHold; }
            set { inHold = value; }
        }

        float timer = 0f;
        float idleInterval = 2000f;
        float interval = 200f;
        float jumpInterval = 100f;
        int currentFrame = 0;
        int spriteWidth = 35;
        int spriteHeight = 130;

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList;
            }
        }

        public UserControlledCharacter()
            : this(Vector2.Zero)
        {
        }

        public UserControlledCharacter(Vector2 position)
        {
            //physics = new GravityPhysics(position, PLAYER_MASS)
            //{
            //    GravityEnabled = true
            //};
            Position = position;
            health = 100;
            direction = new Vector2(1, 0);

            // this seems stupid
            activeItem = 0;
            inventory = new List<IInventoryItem>();
            inventory.Add(new Arrow());
            inventory.Add(new Dart());

            animations = CreateAnimations();
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var soundCache = engineRegistrations.Resolve<IResourceCache<SoundEffect>>();
            jumpSound = soundCache.LoadResource(JUMP_SOUND_NAME);
        }

        public void TakeDamage(float val)
        {

            if (EngineGame.Instance.ScreenManager.CurrentGameplay != null)
            {
                Health -= val;
                EngineGame.Instance.ScreenManager.CurrentGameplay.UpdateHealth(Health);
            }
        }

        public override void OnUpdate(GameTime gameTime, EngineGame game)
        {
            //if (canClimb == null)
            //    TouchingGround = false;

            var start = Physics.Position + new Vector2(0, PhysicsConstants.PixelsToMeters(spriteHeight) / 2);

            game.LevelManager.PhysicsManager.World.RayCast(
                delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
                {
                    if (fixture.Body.UserData is WorldGeometry2)
                    {
                        jumpToggleGuard = true;
                        TouchingGround = true;
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                },
                start,
                start + new Vector2(0, .1f));
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
            // Get the gamepad state.
            var gamepadstate = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            // Get the time scale since the last update call.
            var timeframe = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var amount = 1f;
            var movedirection = new Vector2();

            // Grab the keyboard state.
            var keyboard = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            var d = InputManager.Instance.Pressed(Keys.D);
            var a = InputManager.Instance.Pressed(Keys.A);

            //Update the animation timer by the timeframe in milliseconds
            timer += (timeframe * 1000);

            if (TouchingGround)
                Physics.Friction = 10;
            else
                Physics.Friction = .01f;

            #region Movement
            #region gamepad
            if (gamepad.DPad.Left.Equals(ButtonState.Pressed))
            {
                movedirection.X -= 1.0f;
                if (TouchingGround)
                {
                    currentState = BodyStates.WalkingRight;
                }
            }
            if (gamepad.DPad.Right.Equals(ButtonState.Pressed))
            {
                movedirection.X += 1.0f;
                if (TouchingGround)
                {
                    currentState = BodyStates.WalkingRight;
                }
            }
            if (gamepad.ThumbSticks.Left.X != 0)
            {
                movedirection.X += gamepad.ThumbSticks.Left.X;
                if (TouchingGround)
                {
                    currentState = BodyStates.WalkingRight;
                }
            }
            #endregion

            if (keyboard.IsKeyDown(Keys.A))
            {
                if (currentState == BodyStates.ClimbingBack && canClimb != null && canClimb.VineWall)
                {
                    Physics.Position = new Vector2(Physics.Position.X - PhysicsConstants.PixelsToMeters(5), Physics.Position.Y);
                }
                else if (canClimb != null && canClimb.Sideways && !TouchingGround) //change to sideways climbing state check
                {
                    if (jumpToggleGuard)
                        currentState = BodyStates.NeutralLeft;
                    else
                        movedirection.X -= 1.0f;
                }
                else
                {
                    movedirection.X -= 1.0f;

                    if (TouchingGround)
                    {
                        if (currentState != BodyStates.WalkingLeft)
                        {
                            animations[BodyStates.WalkingLeft].CurrentFrame = 0;
                            currentState = BodyStates.WalkingStartLeft;
                        }
                        else
                        {
                            currentState = BodyStates.WalkingLeft;
                        }
                    }
                    else
                    {
                        currentState = BodyStates.JumpingLeft;
                    }
                }
                //TODO -- add logic for climbing state / animation
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                if (currentState == BodyStates.ClimbingBack && canClimb != null && canClimb.VineWall)
                {
                    Physics.Position = new Vector2(Physics.Position.X + PhysicsConstants.PixelsToMeters(5), Physics.Position.Y);
                }
                else if (canClimb != null && canClimb.Sideways && !TouchingGround) //change to sideways climbing state check
                {
                    if (jumpToggleGuard)
                        currentState = BodyStates.NeutralRight;
                    else
                        movedirection.X += 1.0f;
                }
                else
                {
                    movedirection.X += 1.0f;

                    if (TouchingGround)
                    {
                        if (currentState != BodyStates.WalkingRight)
                        {
                            animations[BodyStates.WalkingRight].CurrentFrame = 0;
                            currentState = BodyStates.WalkingStartRight;
                        }
                        else
                        {
                            currentState = BodyStates.WalkingRight;
                        }
                    }
                    else
                    {
                        currentState = BodyStates.JumpingRight;
                    }
                }
                //TODO -- add logic for climbing state / animation
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                if (canClimb != null)
                {
                    TouchingGround = false;
                    canClimb.Physics.IsSensor = true;
                    Physics.IgnoreGravity = true;
                    Physics.Position = new Vector2(Physics.Position.X, Physics.Position.Y + PhysicsConstants.PixelsToMeters(5));
                }
                else if (TouchingGround)
                {
                    Physics.Friction = .1f;
                    Physics.ApplyLinearImpulse(new Vector2(0, 20));
                }
            }
            #endregion

            #region Direction

            var up = InputManager.Instance.Pressed(Keys.Up);
            var down = InputManager.Instance.Pressed(Keys.Down);
            var right = InputManager.Instance.Pressed(Keys.Right);
            var left = InputManager.Instance.Pressed(Keys.Left);

            if (up && right)
            {
                direction = new Vector2(0.707106769f, -0.707106769f);
                currentState = BodyStates.NeutralRight;
                facing = 1;
            }
            else if (up && left)
            {
                direction = new Vector2(-0.707106769f, -0.707106769f);
                currentState = BodyStates.NeutralLeft;
                facing = -1;
            }
            else if (down && right)
            {
                direction = new Vector2(0.707106769f, 0.707106769f);
                currentState = BodyStates.NeutralRight;
                facing = 1;
            }
            else if (down && left)
            {
                direction = new Vector2(-0.707106769f, 0.707106769f);
                currentState = BodyStates.NeutralLeft;
                facing = -1;
            }
            else if (up)
            {
                direction = new Vector2(0, -1);
            }
            else if (down)
            {
                direction = new Vector2(0, 1);
            }
            else if (right)
            {
                direction = new Vector2(1, 0);
                currentState = BodyStates.NeutralRight;
                facing = 1;
            }
            else if (left)
            {
                direction = new Vector2(-1, 0);
                currentState = BodyStates.NeutralLeft;
                facing = -1;
            }
            else
            {
                direction = new Vector2(1, 0) * facing;
            }


            #endregion

            #region Jumping
            if (keyboard.IsKeyDown(Keys.Space)
                || gamepad.Buttons.A.Equals(ButtonState.Pressed))
            {
                if ((canClimb != null) && !TouchingGround && jumpToggleGuard)
                {
                    Physics.IgnoreGravity = false;
                    Physics.ApplyLinearImpulse(new Vector2(0, -15));
                    jumpToggleGuard = false;
                    //canClimb = false;
                    currentState = BodyStates.JumpingRight;
                }
                if (jumpToggleGuard && TouchingGround)
                {
                    jumpSound.Play();
                    Physics.ApplyLinearImpulse(new Vector2(0, -15));
                    jumpToggleGuard = false;

                    if (currentState == BodyStates.WalkingRight ||
                        currentState == BodyStates.NeutralRight ||
                        currentState == BodyStates.JumpingRight) //Will be changed once we have a landing anim
                    {
                        currentState = BodyStates.JumpingRight;
                        animations[BodyStates.JumpingRight].CurrentFrame = 0;
                    }
                    else if (currentState == BodyStates.WalkingLeft ||
                             currentState == BodyStates.NeutralLeft ||
                             currentState == BodyStates.JumpingLeft)//Will be changed once we have a landing anim
                    {
                        currentState = BodyStates.JumpingLeft;
                        animations[BodyStates.JumpingLeft].CurrentFrame = 0;
                    }
                }
            }
            else if (TouchingGround)
            {
               // jumpToggleGuard = true;
            }

            if (keyboard.IsKeyDown(Keys.W))
            {
                if ((canClimb != null))
                {
                    //Insert anim state change here for climbing anim
                    Physics.LinearVelocity = Vector2.Zero;
                    Physics.IgnoreGravity = true;
                    TouchingGround = false;
                    jumpToggleGuard = true;
                    if (!canClimb.VineWall && !canClimb.Sideways)
                    {                       
                        currentState = BodyStates.ClimbingBack;
                        Physics.Position = new Vector2(canClimb.Position.X,
                                                       Physics.Position.Y - PhysicsConstants.PixelsToMeters(5));
                    }
                    else
                    {
                        if (canClimb.VineWall)
                            currentState = BodyStates.ClimbingBack;
                        else if (canClimb.Sideways)
                            currentState = BodyStates.NeutralRight; //TODO -- Change to sideways climb state
                        Physics.Position = new Vector2(Physics.Position.X,
                                                       Physics.Position.Y - PhysicsConstants.PixelsToMeters(5));
                    }
                }
             /*   else if (canClimb != null)
                {

                    Physics.LinearVelocity = Vector2.Zero;
                    Physics.IgnoreGravity = true;
                    jumpToggleGuard = true;
                    Physics.Position = new Vector2(canClimb.Position.X, Physics.Position.Y - PhysicsConstants.PixelsToMeters(5));
                    if (!canClimb.Sideways)
                        currentState = BodyStates.Climbing;
                }*/ //Dont need this because it's duplicate code -- check jumptoggle though
            }
            #endregion

            #region Shooting

            if (InputManager.Instance.IsNewKey(Keys.F))
            {
                //currentState = BodyStates.ShootingRight;
                holdTime = gameTime.TotalGameTime.TotalSeconds;
                inHold = true;
            }
            else if (!InputManager.Instance.Pressed(Keys.F) && inHold)
            {
                inventory[activeItem].Use(this, world, gameTime, holdTime);
            }

            if (InputManager.Instance.IsNewKey(Keys.G))
            {
                if (activeItem == inventory.Count - 1)
                {
                    activeItem = 0;
                }
                else
                {
                    activeItem++;
                }
            }

            #endregion

            //No keys are pressed and we're on the ground, we're neutral
            if (keyboard.GetPressedKeys().GetLength(0) == 0 && TouchingGround && timer >= interval)
            {
                if (currentState == BodyStates.WalkingRight)
                {
                    currentState = BodyStates.WalkingEndRight;
                    timer = 0f;
                }
                else if (currentState == BodyStates.WalkingLeft)
                {
                    currentState = BodyStates.WalkingEndLeft;
                    timer = 0f;
                }
                else if (currentState == BodyStates.NeutralLeft)
                {
                    animations[BodyStates.IdleRightOpen].CurrentFrame = 0;
                }
                else if (currentState == BodyStates.NeutralRight)
                {
                    animations[BodyStates.IdleRightOpen].CurrentFrame = 0;
                }
                /*  else if (currentState != BodyStates.IdleRightOpen && currentState != BodyStates.WalkingEndRight)
                  {
                      animations[BodyStates.IdleRightOpen].CurrentFrame = 0;
                      currentState = BodyStates.NeutralRight;
                  }*/

                // timer = 0f;
            }

            if (movedirection != Vector2.Zero)
            {
                // Normalize direction to 1.0 magnitude to avoid walking faster at angles.
                movedirection.Normalize();
            }

            // Increment animation unless idle.
            if (amount != 0.0f)
            {
                // Rotate the player towards the controller direction.
                playerRotation = (float)(Math.Atan2(movedirection.Y, movedirection.X) + Math.PI / 2.0);

                // Move player based on the controller direction and time scale.
                Physics.ApplyLinearImpulse(movedirection * amount);
            }

            ClampVelocity();

            UpdateAnimationStates();
        }

        private void ClampVelocity()
        {
            var v = Physics.LinearVelocity;
            if (v.X > X_CLAMP)
                v.X = X_CLAMP;
            else if (v.X < -X_CLAMP)
                v.X = -X_CLAMP;

            if (v.Y > Y_CLAMP)
                v.Y = Y_CLAMP;
            else if (v.Y < -Y_CLAMP)
                v.Y = -Y_CLAMP;

            Physics.LinearVelocity = v;
        }

        protected void UpdateAnimationStates()
        {
            if (currentState == BodyStates.IdleRightOpen && timer >= interval)
            {
                var idle = animations[BodyStates.IdleRightOpen];
                idle.CurrentFrame = (idle.CurrentFrame + 1) % idle.NumFrames;
                timer = 0f;
            }
            else if ((currentState == BodyStates.NeutralRight) && timer >= idleInterval)
            {
                currentState = BodyStates.IdleRightOpen;
                timer = 0f;
            }

            else if (currentState == BodyStates.WalkingRight && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingRight];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingStartRight && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingRight].CurrentFrame = 0;
                currentState = BodyStates.WalkingRight;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingEndRight && timer >= interval)
            {
                currentState = BodyStates.NeutralRight;
                timer = 0f;
            }

            else if (currentState == BodyStates.WalkingLeft && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingLeft];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                facing = -1;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingStartLeft && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingLeft].CurrentFrame = 0;
                currentState = BodyStates.WalkingLeft;
                facing = -1;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingEndLeft && timer >= interval)
            {
                currentState = BodyStates.NeutralLeft;
                timer = 0f;
            }

            if (currentState == BodyStates.JumpingRight && timer >= interval)
            {
                if (!TouchingGround && Physics.LinearVelocity.Y < 0)
                {
                    animations[BodyStates.JumpingRight].CurrentFrame = 1;
                }
                else if (!TouchingGround && Physics.LinearVelocity.Y > 0)
                {
                    animations[BodyStates.JumpingRight].CurrentFrame = 2;
                }
                else if (TouchingGround)
                {
                    animations[BodyStates.JumpingRight].CurrentFrame = 3;
                }

                timer = 0f;
            }

            if (currentState == BodyStates.JumpingLeft && timer >= interval)
            {
                if (!TouchingGround && Physics.LinearVelocity.Y < 0)
                {
                    animations[BodyStates.JumpingLeft].CurrentFrame = 1;
                }
                else if (!TouchingGround && Physics.LinearVelocity.Y > 0)
                {
                    animations[BodyStates.JumpingLeft].CurrentFrame = 2;
                }
                else if (TouchingGround)
                {
                    animations[BodyStates.JumpingLeft].CurrentFrame = 3;
                }

                timer = 0f;
            }
        }

        [OnCollidedWith.Overload]
        public bool OnCollidedWith(WorldGeometry world, Contact info)
        {
            Vector2 normal;
            FixedArray2<Vector2> points;
            info.GetWorldManifold(out normal, out points);

            return true;
        }

        [OnCollidedWith.Overload]
        public bool OnCollidedWith(Vine vine, Contact info)
        {
            vine.VineAnchor.ApplyLinearImpulse(Physics.LinearVelocity);
            var spriteWidthMeters = PhysicsConstants.PixelsToMeters(spriteWidth);
            var spriteHeightMeters = PhysicsConstants.PixelsToMeters(spriteHeight);
            vineAttachment = JointFactory.CreateRevoluteJoint(_world, Physics, vine.VineAnchor, new Vector2(0, vine.TextureHeight / 2));
            //Physics.Position = vine.Position + new Vector2(0, PhysicsConstants.PixelsToMeters((int)vine.TextureHeight));
            //Physics.FixedRotation = false;
            return true;
        }

        public override IRendering Rendering
        {
            get
            {
                var anim = animations[currentState];
                anim.Position = PhysicsConstants.MetersToPixels(Physics.Position);
                return anim;
            }
        }

        private Dictionary<BodyStates, NewAnimationRendering> CreateAnimations()
        {
            var dictionary = new Dictionary<BodyStates, NewAnimationRendering>();

            #region Neutral

            dictionary.Add(
                BodyStates.NeutralRight,
                new NewAnimationRendering(
                    NEUTRAL_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(
                BodyStates.NeutralLeft,
                 new NewAnimationRendering(
                    NEUTRAL_LEFT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            #endregion

            #region Idle

            dictionary.Add(
                BodyStates.IdleRightOpen,
                new NewAnimationRendering(
                        IDLE_OPEN_HAND,
                        new Vector2(76.8f, 153.6f),
                        5,
                        Vector2.Zero,
                        0,
                        Vector2.One));
            dictionary.Add(
                BodyStates.IdleRightClosed,
                new NewAnimationRendering(
                        IDLE_CLOSED_HAND,
                        new Vector2(76.8f, 153.6f),
                        5,
                        Vector2.Zero,
                        0,
                        Vector2.One));

            #endregion

            #region Walking

            dictionary.Add(BodyStates.WalkingStartRight,
                new NewAnimationRendering(
                    WALKING_RIGHT_INTERMEDIATE,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.WalkingRight,
                new NewAnimationRendering(
                    WALKING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.WalkingEndRight,
                new NewAnimationRendering(
                    WALKING_RIGHT_INTERMEDIATE,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.WalkingStartLeft,
                new NewAnimationRendering(
                    WALKING_LEFT_INTERMEDIATE,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.WalkingLeft,
                new NewAnimationRendering(
                    WALKING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.WalkingEndLeft,
                new NewAnimationRendering(
                    WALKING_LEFT_INTERMEDIATE,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            #endregion

            #region Jumping

            dictionary.Add(BodyStates.JumpingRight,
                new NewAnimationRendering(
                    JUMPING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.JumpingLeft,
                new NewAnimationRendering(
                    JUMPING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            #endregion
            #region Climbing
            dictionary.Add(BodyStates.ClimbingBack,
                new NewAnimationRendering(
                    FACING_BACK,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            #endregion


            return dictionary;
        }

        public void RegisterDot(DamageOverTimeEffect dot)
        {
        }

        private const float X_CLAMP = 8;
        private const float Y_CLAMP = 30;

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<World>();
                _world = world;
                Physics = BodyFactory.CreateBody(world, Position, this);

                Width = spriteWidth;
                Height = spriteHeight;
                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                var r = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters - spriteWidthMeters / 2,
                    1.4f,
                    new Vector2(0, -spriteWidthMeters / 4),
                    Physics);
                var c = FixtureFactory.AttachCircle(
                    spriteWidthMeters / 2,
                    1.4f,
                    Physics,
                    new Vector2(0, (spriteHeightMeters - spriteWidthMeters) / 2));

                r.CollidesWith = Category.Cat1;
                r.CollisionCategories = Category.Cat3;
                c.CollidesWith = Category.Cat1;
                c.CollisionCategories = Category.Cat3;
                c.UserData = true;

                var rSens = r.Clone(Physics);
                rSens.IsSensor = true;
                rSens.Shape.Density = 0;

                var cSens = c.Clone(Physics);
                cSens.IsSensor = true;
                cSens.Shape.Density = 0;

                rSens.CollidesWith = Category.All;
                cSens.CollidesWith = Category.All;
                rSens.CollisionCategories = Category.Cat2;
                cSens.CollisionCategories = Category.Cat2;

                Physics.BodyType = BodyType.Dynamic;
                Physics.FixedRotation = true;
                Physics.Friction = 10.0f;

                initialized = true;
            }
        }
    }
}