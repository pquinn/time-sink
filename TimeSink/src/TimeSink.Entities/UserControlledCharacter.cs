﻿using System;
using System.Linq;
using System.Collections.Generic;
using Autofac;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Objects;
using TimeSink.Entities.Weapons;

namespace TimeSink.Entities
{
    [SerializableEntity("defb4f64-1021-420d-8069-e24acebf70bb")]
    public class UserControlledCharacter : Entity, IHaveHealth, IHaveShield, IHaveMana
    {
        const float PLAYER_MASS = 130f;
        const string EDITOR_NAME = "User Controlled Character";

        private static readonly Guid GUID = new Guid("defb4f64-1021-420d-8069-e24acebf70bb");
        enum BodyStates
        {
            NeutralRight, NeutralLeft,
            IdleRightOpen, IdleRightClosed, IdleLeftOpen, IdleLeftClosed,
            WalkingStartRight, WalkingRight, WalkingEndRight, WalkingStartLeft, WalkingLeft, WalkingEndLeft,
            RunningStartRight, RunningRight, RunningStopRight, RunningStartLeft, RunningLeft, RunningStopLeft,
            JumpingRight, JumpingLeft,
            ShootingRight, ShootingLeft,
            DuckingRight, DuckingLeft,
            ClimbingBack,
            ClimbingLeft, ClimbingRight,
            ClimbingLeftNeutral, ClimbingRightNeutral,
            ClimbingLookRight, ClimbingLookLeft,
            HorizontalClimbLeft, HorizontalClimbRight, HorizontalClimbLeftNeut, HorizontalClimbRightNeut 
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
        const string RUNNING_RIGHT = "Textures/Sprites/SpriteSheets/RunningRight";
        const string RUNNING_LEFT = "Textures/Sprites/SpriteSheets/RunningLeft";
        const string RUNNING_RIGHT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Running_Intermediate_Right";
        const string RUNNING_LEFT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Running_Intermediate_Left";
        const string JUMPING_LEFT = "Textures/Sprites/SpriteSheets/JumpingLeft";
        const string FACING_BACK = "Textures/Sprites/SpriteSheets/Backward";
        const string CLIMBING_LEFT = "Textures/Sprites/SpriteSheets/ClimbingLeft";
        const string CLIMBING_RIGHT = "Textures/Sprites/SpriteSheets/ClimbingRight";
        const string CLIMBING_NEUTRAL_LEFT = "Textures/Sprites/SpriteSheets/ClimbingLeftNeut";
        const string CLIMBING_NEUTRAL_RIGHT = "Textures/Sprites/SpriteSheets/ClimbingRightNeut";
        const string CLIMBING_LOOKING_RIGHT = "Textures/Sprites/SpriteSheets/ClimbingLeftLookRight";
        const string CLIMBING_LOOKING_LEFT = "Textures/Sprites/SpriteSheets/ClimbingRightLookLeft";
        const string HORIZ_CLIMBING_LEFT = "Textures/Sprites/SpriteSheets/HorizClimbLeft";
        const string HORIZ_CLIMBING_RIGHT = "Textures/Sprites/SpriteSheets/HorizClimbRight";
        const string HORIZ_CLIMBING_LEFT_NEUT = "Textures/Sprites/SpriteSheets/HorizontalClimbLeftNeut";
        const string HORIZ_CLIMBING_RIGHT_NEUT = "Textures/Sprites/SpriteSheets/HorizontalClimbRightNeut";

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
        private bool climbing = false;
        private World _world;
        private float origDamping;

        private Joint vineAttachment;

        public Ladder CanClimb { get { return canClimb; } set { canClimb = value; } }
        public bool Climbing {get {return climbing;} set {climbing = value;}}

        private List<IInventoryItem> inventory;
        public override IMenuItem InventoryItem { get { return inventory[activeItem]; } }
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
        public  Vector2 Direction
        {
            get { return direction; }
             set { direction = value; }
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
        public  bool InHold
        {
            get { return inHold; }
            set { inHold = value; }
        }

        float timer = 0f;
        float idleInterval = 2000f;
        float interval = 200f;
        int currentFrame = 0;
        int spriteWidth = 35;
        int spriteHeight = 130;
        bool isRunning;
        public Body WheelBody { get; set; }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList
                    .Concat(WheelBody.FixtureList)
                    .ToList();
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
            //Console.WriteLine("Character Position: {0}", Position);
            //Console.WriteLine("Previous Position: {0}", PreviousPosition);
            //Console.WriteLine();


            if (!BridgeHanging())
                TouchingGround = false;

            var start = Physics.Position + new Vector2(0, PhysicsConstants.PixelsToMeters(spriteHeight) / 2);


            game.LevelManager.PhysicsManager.World.RayCast(
                delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
                {
                    if (fixture.Body.UserData is WorldGeometry2 || fixture.Body.UserData is MovingPlatform)
                    {
                        jumpToggleGuard = true;
                        TouchingGround = true;
                        return 0;
                    }
                    else if (fixture.Body.UserData is Ladder)
                    {
                        TouchingGround = true;
                        jumpToggleGuard = true;
                        Climbing = true;
                        fixture.Body.IsSensor = false;
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
            var climbAmount = 4f;
            var movedirection = new Vector2();

            // Grab the keyboard state.
            var keyboard = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            var d = InputManager.Instance.Pressed(Keys.D);
            var a = InputManager.Instance.Pressed(Keys.A);

            //Update the animation timer by the timeframe in milliseconds
            timer += (timeframe * 1000);

            if (TouchingGround)
                Physics.Friction = WheelBody.Friction = 10;
            else
                Physics.Friction = WheelBody.Friction = .01f;

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

            if (InputManager.Instance.Pressed(Keys.LeftShift))
            {
                isRunning = true;
            }
            else
                isRunning = false;

            if (keyboard.IsKeyDown(Keys.A))
            {
                facing = -1;
                if (ClimbingState())
                {
                    Physics.LinearVelocity = WheelBody.LinearVelocity = Vector2.Zero;
                }

                if (currentState == BodyStates.ClimbingBack && canClimb != null && canClimb.VineWall)
                {
                    movedirection.X -= 1.0f;// Physics.Position = new Vector2(Physics.Position.X - PhysicsConstants.PixelsToMeters(5), Physics.Position.Y);
                    Physics.LinearDamping = 5f;
                }

                else if (currentState == BodyStates.ClimbingBack)
                {
                    // Do Nothing -- Normal Ladder
                }
                else if (RightFacingBodyState() && ClimbingState())
                {
                    currentState = BodyStates.ClimbingLookLeft;
                }
                else if (LeftFacingBodyState() && ClimbingState())
                {
                    currentState = BodyStates.ClimbingLeftNeutral;
                }
                else
                {
                    if (!swinging || Physics.LinearVelocity.X <= 0)
                        movedirection.X -= 1.0f;

                    if (TouchingGround)
                    {
                        if (isRunning)
                        {
                            if (currentState != BodyStates.RunningLeft)
                            {
                                animations[BodyStates.RunningLeft].CurrentFrame = 0;
                                currentState = BodyStates.RunningStartLeft;
                            }
                        }
                        else if (currentState != BodyStates.WalkingLeft)
                        {
                            animations[BodyStates.WalkingLeft].CurrentFrame = 0;
                            currentState = BodyStates.WalkingStartLeft;
                        }
                        else
                        {
                            currentState = BodyStates.WalkingLeft;
                        }
                    }
                    else if (currentState == BodyStates.HorizontalClimbLeft || currentState == BodyStates.HorizontalClimbRight ||
                            currentState == BodyStates.HorizontalClimbLeftNeut || currentState == BodyStates.HorizontalClimbRightNeut)
                    {
                        currentState = BodyStates.HorizontalClimbLeft;
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
                facing = 1;

                if (ClimbingState())
                {
                    Physics.LinearVelocity = WheelBody.LinearVelocity = Vector2.Zero;
                }

                if (currentState == BodyStates.ClimbingBack && canClimb != null && canClimb.VineWall)
                {
                    movedirection.X += 1.0f;// Physics.Position = new Vector2(Physics.Position.X + PhysicsConstants.PixelsToMeters(5), Physics.Position.Y);
                    Physics.LinearDamping = 5f;
                }
                else if (currentState == BodyStates.ClimbingBack)
                {
                    // Do Nothing
                }
                else if (LeftFacingBodyState() && ClimbingState())
                {
                    currentState = BodyStates.ClimbingLookRight;
                }
                else if (RightFacingBodyState() && ClimbingState())
                {
                    currentState = BodyStates.ClimbingRightNeutral;
                }
                else
                {
                    if (!swinging || Physics.LinearVelocity.X >= 0)
                        movedirection.X += 1.0f;

                    if (TouchingGround)
                    {
                        if (isRunning)
                        {
                            if (currentState != BodyStates.RunningRight)
                            {
                                animations[BodyStates.RunningRight].CurrentFrame = 0;
                                currentState = BodyStates.RunningStartRight;
                            }
                        }
                        else if (currentState != BodyStates.WalkingRight)
                        {
                            animations[BodyStates.WalkingRight].CurrentFrame = 0;
                            currentState = BodyStates.WalkingStartRight;
                        }
                        else
                        {
                            currentState = BodyStates.WalkingRight;
                        }
                    }
                    else if (currentState == BodyStates.HorizontalClimbLeft || currentState == BodyStates.HorizontalClimbRight ||
                            currentState == BodyStates.HorizontalClimbLeftNeut || currentState == BodyStates.HorizontalClimbRightNeut)
                    {
                        currentState = BodyStates.HorizontalClimbRight;
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
                #region Climbing

                if (ClimbingState())
                {
                    Physics.LinearVelocity = WheelBody.LinearVelocity = Vector2.Zero;
                }
                if (canClimb != null)
                {
                    TouchingGround = false;
                    canClimb.Physics.IsSensor = true;
                    Physics.IgnoreGravity = WheelBody.IgnoreGravity = true;

                    if (!canClimb.Sideways)
                        currentState = BodyStates.ClimbingBack;
                    else if (RightFacingBodyState())
                        currentState = BodyStates.ClimbingRight;
                    else
                        currentState = BodyStates.ClimbingLeft;
                   /*
                    var v = new Vector2(0, PhysicsConstants.PixelsToMeters(5));
                    Physics.Position += v;
                    WheelBody.Position += v;
                    */
                    movedirection.Y += 1.0f;
                    Physics.LinearDamping = 5f;
                }
                #endregion
                //Sliding
                else if (TouchingGround)
                {
                    Physics.Friction = WheelBody.Friction = .1f;
                    WheelBody.ApplyLinearImpulse(new Vector2(0, 20));
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
            if (InputManager.Instance.IsNewKey(Keys.Space)
                || gamepad.Buttons.A.Equals(ButtonState.Pressed))
            {
                if (BridgeHanging())
                {
                    vineBridge.ForceSeperation(this);
                    if (!InputManager.Instance.Pressed(Keys.S))
                        PerformJump();
                }
                else if (swinging)
                {
                    ForceVineSeperate();
                    PerformJump(.5f);
                }
                else if ((canClimb != null) && !TouchingGround && jumpToggleGuard)
                {
                    Physics.LinearDamping = canClimb.LinearDamping;
                    Physics.IgnoreGravity = WheelBody.IgnoreGravity = false;
                    PerformJump();
                }
                else if (jumpToggleGuard && TouchingGround)
                {
                    PerformJump();
                }
            }
            if (keyboard.IsKeyDown(Keys.S) && InputManager.Instance.IsNewKey(Keys.Space))
            {
                if (TouchingGround)
                {
                    PerformJump();
                }
            }
            #endregion

            #region climbing
            if (keyboard.IsKeyDown(Keys.W))
            {
                if ((canClimb != null))
                {
                    Climbing = true;
                    //Insert anim state change here for climbing anim
                    Physics.LinearVelocity = WheelBody.LinearVelocity = Vector2.Zero;
                    Physics.IgnoreGravity = WheelBody.IgnoreGravity = true;
                    TouchingGround = false;
                    jumpToggleGuard = true;
                    if (!canClimb.VineWall)
                    {
                        if (canClimb.Sideways)
                        {
                            if (RightFacingBodyState())
                                currentState = BodyStates.ClimbingRight; //TODO -- Change to sideways climb state
                            else if (LeftFacingBodyState())
                                currentState = BodyStates.ClimbingLeft;


                            if (Physics.Position.X > canClimb.Position.X) //We are to the right of the ladder
                            {
                                Physics.Position = new Vector2(Physics.Position.X - ((Physics.Position.X - canClimb.Position.X) / 2),
                                                               Physics.Position.Y);

                                WheelBody.Position = new Vector2(WheelBody.Position.X - ((WheelBody.Position.X - canClimb.Position.X) / 2),
                                                               WheelBody.Position.Y);
                                movedirection.Y -= 1.0f;
                                Physics.LinearDamping = 5f;
                            }
                            else if (Physics.Position.X < canClimb.Position.X) //We are to the left of the ladder
                            {
                                Physics.Position = new Vector2(Physics.Position.X + ((canClimb.Position.X - Physics.Position.X) / 2),
                                                               Physics.Position.Y);
                                WheelBody.Position = new Vector2(WheelBody.Position.X + ((canClimb.Position.X - WheelBody.Position.X) / 2),
                                                               WheelBody.Position.Y);
                                movedirection.Y -= 1.0f;
                                Physics.LinearDamping = 5f;
                            }
                        }

                        else
                        {
                            currentState = BodyStates.ClimbingBack;
                            Physics.Position = new Vector2(canClimb.Position.X,
                                                           Physics.Position.Y);

                            WheelBody.Position = new Vector2(canClimb.Position.X,
                                                           WheelBody.Position.Y);
                            movedirection.Y -= 1.0f;
                            Physics.LinearDamping = 5f;
                        }
                    }
                    else
                    {
                        currentState = BodyStates.ClimbingBack;
                        Physics.Position = new Vector2(Physics.Position.X,
                                                       Physics.Position.Y);

                        WheelBody.Position = new Vector2(WheelBody.Position.X,
                                                       WheelBody.Position.Y);
                        movedirection.Y -= 1.0f;
                        Physics.LinearDamping = 5f;
                    }                    
                }
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
                EngineGame.Instance.ScreenManager.CurrentGameplay.UpdatePrimaryItems(this);
            }

            #endregion

            //No keys are pressed and we're on the ground, we're neutral
            if(keyboard.GetPressedKeys().GetLength(0) == 0)
            {
                if (TouchingGround && timer >= interval)
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
                    if (currentState == BodyStates.RunningLeft)
                    {
                        currentState = BodyStates.RunningStopLeft;
                        timer = 0f;
                    }
                    if (currentState == BodyStates.RunningRight)
                    {
                        currentState = BodyStates.RunningStopRight;
                        timer = 0f;
                    }
                }
                //Set to climbing neutral states
                if (LeftFacingBodyState() && ClimbingState())
                {
                    currentState = BodyStates.ClimbingLeftNeutral;
                }
                else if (RightFacingBodyState() && ClimbingState())
                {
                    currentState = BodyStates.ClimbingRightNeutral;
                }
                else if (currentState == BodyStates.HorizontalClimbRight)
                {
                    currentState = BodyStates.HorizontalClimbRightNeut;
                }
                else if (currentState == BodyStates.HorizontalClimbLeft)
                {
                    currentState = BodyStates.HorizontalClimbLeftNeut;
                }
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

                if (!ClimbingState())
                {
                    // Move player based on the controller direction and time scale.
                    Physics.ApplyLinearImpulse(movedirection * amount);
                }
                else
                    Physics.ApplyLinearImpulse(movedirection * climbAmount);

                MotorJoint.MotorSpeed = movedirection.X * 10;
            }

            ClampVelocity();

            UpdateAnimationStates();
        }

        private bool BridgeHanging()
        {
            return vineBridge != null && vineBridge.Hanging;
        }

        private void PerformJump(float percentOfMax = 1)
        {
            jumpSound.Play();
            Physics.ApplyLinearImpulse(new Vector2(0, -23.5f * percentOfMax));
            jumpToggleGuard = false;

            if (facing > 0)
            {
                currentState = BodyStates.JumpingRight;
                animations[BodyStates.JumpingRight].CurrentFrame = 0;
            }
            else
            {
                currentState = BodyStates.JumpingLeft;
                animations[BodyStates.JumpingLeft].CurrentFrame = 0;
            }
        }

        private const float WALK_X_CLAMP = 4;
        private const float WALK_Y_CLAMP = 15;
        private const float RUN_X_CLAMP = 10;
        private const float SWING_X_CLAMP = 8;

        private void ClampVelocity()
        {
            float x_vel = WALK_X_CLAMP;

            if (swinging)
                x_vel = SWING_X_CLAMP;
            else if (isRunning)
            {
                x_vel = RUN_X_CLAMP;
                if (!TouchingGround)
                {
                    x_vel = x_vel * .8f;
                }
            }

            var v = Physics.LinearVelocity;
            if (v.X > x_vel)
                v.X = x_vel;
            else if (v.X < -x_vel)
                v.X = -x_vel;

            if (v.Y > WALK_Y_CLAMP)
                v.Y = WALK_Y_CLAMP;
            else if (v.Y < -WALK_Y_CLAMP)
                v.Y = -WALK_Y_CLAMP;

            Physics.LinearVelocity = v;

            v = WheelBody.LinearVelocity;
            if (v.X > x_vel)
                v.X = x_vel;
            else if (v.X < -x_vel)
                v.X = -x_vel;

            if (v.Y > WALK_Y_CLAMP)
                v.Y = WALK_Y_CLAMP;
            else if (v.Y < -WALK_Y_CLAMP)
                v.Y = -WALK_Y_CLAMP;

            WheelBody.LinearVelocity = v;
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
            else if ((currentState == BodyStates.WalkingEndRight ||
                      currentState == BodyStates.RunningStopRight) && timer >= interval)
            {
                currentState = BodyStates.NeutralRight;
                timer = 0f;
            }

            else if ((currentState == BodyStates.WalkingEndLeft ||
                      currentState == BodyStates.RunningStopLeft) && timer >= interval)
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

            else if (currentState == BodyStates.RunningStartRight && timer >= interval)
            {
                var walking = animations[BodyStates.RunningRight].CurrentFrame = 0;
                currentState = BodyStates.RunningRight;
                timer = 0f;
            }
            else if (currentState == BodyStates.RunningStartLeft && timer >= interval)
            {
                var walking = animations[BodyStates.RunningLeft].CurrentFrame = 0;
                currentState = BodyStates.RunningLeft;
                timer = 0f;
            }
            else if (currentState == BodyStates.RunningRight && timer >= interval)
            {
                var walking = animations[BodyStates.RunningRight];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                timer = 0f;
            }
            else if (currentState == BodyStates.RunningLeft && timer >= interval)
            {
                var walking = animations[BodyStates.RunningLeft];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
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
                else if (animations[BodyStates.JumpingRight].CurrentFrame == 3)
                {
                    currentState = BodyStates.NeutralRight;
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
                else if (animations[BodyStates.JumpingLeft].CurrentFrame == 3)
                {
                    currentState = BodyStates.NeutralLeft;
                }
                else if (TouchingGround)
                {
                    animations[BodyStates.JumpingLeft].CurrentFrame = 3;
                }

                timer = 0f;
            }
            if (currentState == BodyStates.ClimbingLeft && timer >= interval)
            {
                var climbing = animations[BodyStates.ClimbingLeft];
                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                facing = -1;
                timer = 0f;
            }
            if (currentState == BodyStates.ClimbingRight && timer >= interval)
            {
                var climbing = animations[BodyStates.ClimbingRight];
                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                timer = 0f;
            }
            if (currentState == BodyStates.HorizontalClimbLeft && timer >= interval)
            {
                var climbing = animations[BodyStates.HorizontalClimbLeft];
                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                facing = -1;
                timer = 0f;
            }
            if (currentState == BodyStates.HorizontalClimbRight && timer >= interval)
            {
                var climbing = animations[BodyStates.HorizontalClimbRight];
                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                timer = 0f;
            }
        }

        [OnCollidedWith.Overload]
        public bool OnCollidedWith(WorldGeometry2 world, Contact info)
        {
            Vector2 normal;
            FixedArray2<Vector2> points;
            info.GetWorldManifold(out normal, out points);

            return true;
        }

        private VineBridge vineBridge;
        [OnCollidedWith.Overload]
        public bool OnCollidedWith(VineBridge bridge, Contact info)
        {
            if (LeftFacingBodyState())
                currentState = BodyStates.HorizontalClimbLeft;
            else
                currentState = BodyStates.HorizontalClimbRight;

            vineBridge = bridge;

            return true;
        }

        [OnSeparation.Overload]
        public void OnSeparation(Fixture f1, VineBridge bridge, Fixture f2)
        {
            if (LeftFacingBodyState())
                currentState = BodyStates.JumpingLeft;
            else
                currentState = BodyStates.JumpingRight;
        }

        private WeldJoint vineJoint;
        private bool swinging;
        private bool leftVine = true;
        [OnCollidedWith.Overload]
        public bool OnCollidedWith(Vine vine, Contact info)
        {
            if (!swinging && leftVine)
            {
                var pointOnVine = new Vector2(0, PhysicsConstants.PixelsToMeters((int)(vine.Height * .4)));

                Position = vine.VineAnchor.GetWorldPoint(pointOnVine);
                vineJoint = JointFactory.CreateWeldJoint(
                    vine.VineAnchor,
                    Physics,
                    Vector2.Zero);
                Physics.FixedRotation = false;
                vine.VineAnchor.ApplyLinearImpulse(Physics.LinearVelocity, Physics.Position);

                _world.AddJoint(vineJoint);
                swinging = true;
                leftVine = false;
            }

            return true;
        }

        [OnSeparation.Overload]
        public void OnSeparation(Fixture f1, Vine vine, Fixture f2)
        {
            leftVine = true;
        }

        private void ForceVineSeperate()
        {
            Physics.AngularVelocity = 0;
            Physics.Rotation = 0;
            Physics.FixedRotation = true;
            //Physics.ApplyLinearImpulse(vineJoint.BodyA.LinearVelocity);
            _world.RemoveJoint(vineJoint);
            swinging = false;
        }

        public override IRendering Rendering
        {
            get
            {
                var anim = animations[currentState];
                anim.Position = PhysicsConstants.MetersToPixels(Physics.Position);
                anim.Rotation = Physics.Rotation;
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

            #region Running
            dictionary.Add(BodyStates.RunningStopLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT_INTERMEDIATE,
                    new Vector2(153.6f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.RunningStopRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT_INTERMEDIATE,
                    new Vector2(153.6f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.RunningStartLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT_INTERMEDIATE,
                    new Vector2(153.6f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.RunningStartRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT_INTERMEDIATE,
                    new Vector2(153.6f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.RunningLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT,
                    new Vector2(153.6f, 153.6f),
                    8,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.RunningRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT,
                    new Vector2(153.6f, 153.6f),
                    8,
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

            dictionary.Add(BodyStates.ClimbingLeft,
               new NewAnimationRendering(
                    CLIMBING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.ClimbingRight,
               new NewAnimationRendering(
                    CLIMBING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.ClimbingRightNeutral,
               new NewAnimationRendering(
                    CLIMBING_NEUTRAL_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.ClimbingLeftNeutral,
               new NewAnimationRendering(
                    CLIMBING_NEUTRAL_LEFT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.ClimbingLookRight,
               new NewAnimationRendering(
                    CLIMBING_LOOKING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.ClimbingLookLeft,
               new NewAnimationRendering(
                    CLIMBING_LOOKING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.HorizontalClimbLeft,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.HorizontalClimbRight,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            dictionary.Add(BodyStates.HorizontalClimbRightNeut,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_RIGHT_NEUT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));

            dictionary.Add(BodyStates.HorizontalClimbLeftNeut,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_LEFT_NEUT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One));
            #endregion


            return dictionary;
        }

        private Dictionary<BodyStates, Body> CreatePhysics()
        {
            var dictionary = new Dictionary<BodyStates, Body>();

            Width = spriteWidth;
            Height = spriteHeight;
            float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
            float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);


            #region Neutral

            //newBody = 

            #endregion

            return dictionary;
        }

        public bool RightFacingBodyState()
        {
            return (currentState == BodyStates.ClimbingRight ||
                    currentState == BodyStates.ClimbingRightNeutral ||
                    currentState == BodyStates.ClimbingLookLeft ||
                    currentState == BodyStates.WalkingEndRight ||
                    currentState == BodyStates.WalkingStartRight ||
                    currentState == BodyStates.WalkingRight ||
                    currentState == BodyStates.ShootingRight ||
                    currentState == BodyStates.RunningRight ||
                    currentState == BodyStates.NeutralRight ||
                    currentState == BodyStates.IdleRightOpen ||
                    currentState == BodyStates.IdleRightClosed ||
                    currentState == BodyStates.DuckingRight ||
                    currentState == BodyStates.JumpingRight);
        }
        public bool LeftFacingBodyState()
        {
            return (currentState == BodyStates.ClimbingLeft ||
                    currentState == BodyStates.ClimbingLeftNeutral ||
                    currentState == BodyStates.ClimbingLookRight ||
                    currentState == BodyStates.WalkingEndLeft ||
                    currentState == BodyStates.WalkingStartLeft ||
                    currentState == BodyStates.WalkingLeft ||
                    currentState == BodyStates.ShootingLeft ||
                    currentState == BodyStates.RunningLeft ||
                    currentState == BodyStates.NeutralLeft ||
                    currentState == BodyStates.IdleLeftOpen ||
                    currentState == BodyStates.IdleLeftClosed ||
                    currentState == BodyStates.DuckingLeft ||
                    currentState == BodyStates.JumpingLeft);
        }
        public bool ClimbingState()
        {
            return (currentState == BodyStates.ClimbingBack ||
                    currentState == BodyStates.ClimbingLeft ||
                    currentState == BodyStates.ClimbingRight ||
                    currentState == BodyStates.ClimbingLeftNeutral ||
                    currentState == BodyStates.ClimbingRightNeutral ||
                    currentState == BodyStates.ClimbingLookLeft ||
                    currentState == BodyStates.ClimbingLookRight);
        }

        public void DismountLadder()
        {
            if (RightFacingBodyState())
                currentState = BodyStates.JumpingRight;
            else if (LeftFacingBodyState())
                currentState = BodyStates.JumpingLeft;
            else
                currentState = BodyStates.JumpingRight;
        }


        public void RegisterDot(DamageOverTimeEffect dot)
        {
        }

        private bool initialized;
        private RevoluteJoint MotorJoint;
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

                var wPos = Position + new Vector2(0, (spriteHeightMeters - spriteWidthMeters) / 2);
                WheelBody = BodyFactory.CreateBody(
                    world,
                    wPos,
                    this);

                var c = FixtureFactory.AttachCircle(
                    spriteWidthMeters / 2,
                    1.4f,
                    WheelBody);

                r.CollidesWith = Category.Cat1;
                r.CollisionCategories = Category.Cat3;
                c.CollidesWith = Category.Cat1 | Category.Cat31;
                c.CollisionCategories = Category.Cat3;
                c.UserData = true;
                r.UserData = false;

                var rSens = r.Clone(r.Body);
                var cSens = c.Clone(c.Body);

                MotorJoint = JointFactory.CreateRevoluteJoint(world, Physics, WheelBody, Vector2.Zero);
                MotorJoint.MotorEnabled = true;
                MotorJoint.MaxMotorTorque = 10;

                rSens.IsSensor = true;
                rSens.Shape.Density = 0;
                cSens.IsSensor = true;
                cSens.Shape.Density = 0;

                rSens.CollidesWith = Category.All;
                cSens.CollidesWith = Category.All;
                rSens.CollisionCategories = Category.Cat2;
                cSens.CollisionCategories = Category.Cat2;

                Physics.BodyType = BodyType.Dynamic;
                Physics.FixedRotation = true;
                Physics.Friction = 10.0f;
                WheelBody.BodyType = BodyType.Dynamic;
                WheelBody.Friction = 10.0f;

                var ropeSensor = FixtureFactory.AttachCircle(
                .1f, 5, Physics, new Vector2(0, -(PhysicsConstants.PixelsToMeters(Height) / 4)));
                ropeSensor.Friction = 5f;
                ropeSensor.Restitution = 1f;
                ropeSensor.UserData = this;
                ropeSensor.IsSensor = true;
                ropeSensor.CollidesWith = Category.Cat4;
                ropeSensor.CollisionCategories = Category.Cat4;

                var vineSensor = FixtureFactory.AttachCircle(.1f, 5, Physics, Vector2.Zero);
                vineSensor.Friction = 5f;
                vineSensor.Restitution = 1f;
                vineSensor.UserData = this;
                vineSensor.IsSensor = true;
                vineSensor.CollidesWith = Category.Cat5;
                vineSensor.CollisionCategories = Category.Cat5;

                //var vineSensor = BodyFactory.CreateCircle(
                //    world, .1f, 5,
                //    Physics.Position, this);
                //vineSensor.Friction = 5f;
                //vineSensor.Restitution = 1f;
                //vineSensor.UserData = this;
                //vineSensor.IsSensor = true;
                //vineSensor.CollidesWith = Category.Cat5;
                //vineSensor.CollisionCategories = Category.Cat5;

                //var vineSensorJoint = JointFactory.CreateWeldJoint(world, vineSensor, Physics, new Vector2(0, Height / 2));

                initialized = true;
            }
        }
    }
}