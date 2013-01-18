using System;
using System.Linq;
using System.Collections.Generic;
using Autofac;
using log4net;
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
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserControlledCharacter));
        private double nextLogTime = 0;
        private readonly double LOG_INTERVAL = 5000; //5 seconds = 5000 milliseconds

        const float PLAYER_MASS = 130f;
        const string EDITOR_NAME = "User Controlled Character";

        private static readonly Guid GUID = new Guid("defb4f64-1021-420d-8069-e24acebf70bb");
        enum BodyStates
        {
            #region neutral
            NeutralRight, NeutralLeft,
            IdleRightOpen, IdleRightClosed, IdleLeftOpen, IdleLeftClosed,
            #endregion
            #region walking
            WalkingStartRight, WalkingRight, WalkingEndRight, WalkingStartLeft, WalkingLeft, WalkingEndLeft,
            WalkingShootRight, WalkingShoot2Right, WalkingShoot3Right, WalkingDrawnRight,
            WalkingShootLeft, WalkingShoot2Left, WalkingShoot3Left, WalkingDrawnLeft,
            #endregion
            #region running
            RunningStartRight, RunningRight, RunningStopRight, RunningStartLeft, RunningLeft, RunningStopLeft,
            #endregion
            #region jumping
            JumpingRight, JumpingLeft,
            #endregion
            #region shooting
            ShootingArrowRight, ShootingArrowLeft,
            ShootingArrowNeutRight, ShootingArrowNeutLeft,
            #endregion
            #region ducking
            DuckingRight, DuckingLeft, DuckingRightBow, DuckingLeftBow, DuckShootRightBow, DuckShootLeftBow,
            #endregion
            #region knockback
            KnockbackRight, KnockbackLeft,
            #endregion
            #region climbing
            ClimbingBack, ClimbingBackNeut,
            ClimbingLeft, ClimbingRight,
            ClimbingLeftNeutral, ClimbingRightNeutral,
            ClimbingLookRight, ClimbingLookLeft,
            HorizontalClimbLeft, HorizontalClimbRight, HorizontalClimbLeftNeut, HorizontalClimbRightNeut
            #endregion
        };


        BodyStates currentState;

        //Texture strings for content loading
        const string JUMP_SOUND_NAME = "Audio/Sounds/Hop";

        const string EDITOR_PREVIEW = "Textures/Body_Neutral";

        #region neutral
        const string NEUTRAL_RIGHT = "Textures/Sprites/SpriteSheets/Body_Neutral";
        const string NEUTRAL_LEFT = "Textures/Sprites/SpriteSheets/Neutral_Left";
        const string IDLE_CLOSED_HAND = "Textures/Sprites/SpriteSheets/Idle_OpenHand";
        const string IDLE_OPEN_HAND = "Textures/Sprites/SpriteSheets/Idle_OpenHand";
        #endregion
        #region walking
        const string WALKING_RIGHT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Walking_Right_Intermediate";
        const string WALKING_RIGHT = "Textures/Sprites/SpriteSheets/Body_Walking_Right";
        const string WALKING_LEFT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Walking_Intermediate_Left";
        const string WALKING_LEFT = "Textures/Sprites/SpriteSheets/BodyWalkLeft";
        #endregion
        #region Running
        const string RUNNING_RIGHT = "Textures/Sprites/SpriteSheets/RunningRight";
        const string RUNNING_LEFT = "Textures/Sprites/SpriteSheets/RunningLeft";
        const string RUNNING_RIGHT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Running_Intermediate_Right";
        const string RUNNING_LEFT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Running_Intermediate_Left";
        #endregion
        #region jumping
        const string JUMPING_LEFT = "Textures/Sprites/SpriteSheets/JumpingLeft";
        const string JUMPING_RIGHT = "Textures/Sprites/SpriteSheets/Jumping_Right";
        #endregion
        const string FACING_BACK = "Textures/Sprites/SpriteSheets/Backward";
        #region Knockback
        const string KNOCKBACK_RIGHT = "Textures/Sprites/SpriteSheets/KnockBackRight";
        const string KNOCKBACK_LEFT = "Textures/Sprites/SpriteSheets/KnockBackLeft";
        #endregion
        #region Ducking
        const string DUCK_LEFT = "Textures/Sprites/SpriteSheets/DuckingLeft";
        const string DUCK_RIGHT = "Textures/Sprites/SpriteSheets/DuckingRight";
        const string DUCK_LEFT_BOW = "Textures/Sprites/SpriteSheets/DuckLeftBow";
        const string DUCK_RIGHT_BOW = "Textures/Sprites/SpriteSheets/DuckingRightBow";
        const string DUCK_LEFT_SHOOT_BOW = "Textures/Sprites/SpriteSheets/DuckLeftShoot";
        const string DUCK_RIGHT_SHOOT_BOW = "Textures/Sprites/SpriteSheets/DuckShootRightBow";
        #endregion
        #region climbing
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
        const string CLIMBING_BACK = "Textures/Sprites/SpriteSheets/ClimbingBack";
        const string CLIMBING_NEUT = "Textures/Sprites/SpriteSheets/ClimbingNeut";
        #endregion
        const string SHOOT_ARROW_RIGHT = "Textures/Sprites/SpriteSheets/ShootArrowRight";
        const string SHOOT_ARROW_LEFT = "Textures/Sprites/SpriteSheets/ShootArrowLeft";
        #region walking+shooting
        const string WALK_SHOOT_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShootLeft";
        const string WALK_SHOOT_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShootRight";
        const string WALK_SHOOT2_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShoot2Left";
        const string WALK_SHOOT2_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShoot2Right";
        const string WALK_SHOOT3_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShoot3Left";
        const string WALK_SHOOT3_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShoot3Right";
        const string WALK_DRAWN_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkRightDrawn";
        const string WALK_DRAWN_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkLeftDrawn";
        const string SHOOT_NEUT_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/Body_Neutral_Shooting_Left";
        const string SHOOT_NEUT_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/Body_Neutral_Shooting_Right";
        #endregion

        private Dictionary<BodyStates, NewAnimationRendering> animations;

        const float MAX_ARROW_HOLD = 1;
        const float MIN_ARROW_INIT_SPEED = 500;
        const float MAX_ARROW_INIT_SPEED = 1500;
        public static float X_OFFSET = PhysicsConstants.PixelsToMeters(-5);
        public static float Y_OFFSET = PhysicsConstants.PixelsToMeters(-30);

        private SoundEffect jumpSound;
        private bool jumpToggleGuard = true;
        private Rectangle sourceRect;
        private float health;
        private float mana;
        private float shield;
        private Ladder canClimb = null;
        private bool climbing = false;
        private TorchGround onTorchGround;
        public Torch HoldingTorch {get; set;}
        private World _world;
        private IInventoryItem onPickup;
        public IInventoryItem OnPickup { get { return onPickup; } set { onPickup = value; } }
        public HashSet<DamageOverTimeEffect> Dots { get; set; }

        private Joint vineAttachment;

        public Ladder CanClimb { get { return canClimb; } set { canClimb = value; } }
        public bool Climbing {get {return climbing;} set {climbing = value;}}

        private List<IInventoryItem> inventory;
        public override IMenuItem InventoryItem
        {
            get
            {
                if (inventory.Count != 0)
                    return inventory[activeItem];
                else 
                    return null;
            }
        }
        private int activeItem;
        private ItemPopup currentItemPrompt;

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
                    PhysicsConstants.MetersToPixels(Position),
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
        float shotTimer = 0f;
        float invulnTimer = 0f;
        float damageTimer = 0f;
        const float invulnInterval = 2000f;
        const float damageFlashInterval = 300f;
        float idleInterval = 2000f;
        float interval = 200f;
        float bowInterval = 150f;
        float shotInterval = 500f;
        int currentFrame = 0;
        int spriteWidth = 35;
        int spriteHeight = 130;
        bool isRunning;
        bool isDucking;
        bool invulnerable = false;
        bool invulnFlash = false;
        bool damageFlash = false;
        public bool Invulnerable { get { return invulnerable; } set { invulnerable = value; } }
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
            //inventory.Add(new Dart());

            animations = CreateAnimations();

            Dots = new HashSet<DamageOverTimeEffect>();
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
                if (!Invulnerable)
                {
                    Invulnerable = true;
                    damageFlash = true;
                }
                if (RightFacingBodyState())
                {
                    currentState = BodyStates.KnockbackRight;
                    Physics.ApplyLinearImpulse(new Vector2(-25, 0));
                }
                else if (LeftFacingBodyState())
                {
                    currentState = BodyStates.KnockbackLeft;
                    Physics.ApplyLinearImpulse(new Vector2(25, 0));
                }

                if (!Invulnerable)
                {
                    Health -= val;
                    EngineGame.Instance.ScreenManager.CurrentGameplay.UpdateHealth(Health);
                    Logger.Info(String.Format("Player took {0} damage.", val));
                }
            }
        }

        public override void OnUpdate(GameTime gameTime, EngineGame game)
        {
            RemoveInactiveDots();

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
                        Climbing = false;
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

            foreach (DamageOverTimeEffect dot in Dots)
            {
                if (dot.Active)
                    TakeDamage(dot.Tick(gameTime));
            }

            if (gameTime.TotalGameTime.TotalMilliseconds >= nextLogTime)
            {
                LogMetricSnapshot();
                nextLogTime = gameTime.TotalGameTime.TotalMilliseconds + LOG_INTERVAL;
            }
        }

        private void RemoveInactiveDots()
        {
            Dots.RemoveWhere(x => x.Finished);
        }

        public void AddInventoryItem(IInventoryItem item)
        {
            inventory.Add(item);
            activeItem = inventory.IndexOf(item);
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
            // Get the gamepad state.
            var gamepadstate = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            // Get the time scale since the last update call.
            var timeframe = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var amount = 1f;
            var climbAmount = 6f;
            var movedirection = new Vector2();

            // Grab the keyboard state.
            var keyboard = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            var d = InputManager.Instance.Pressed(Keys.D);
            var a = InputManager.Instance.Pressed(Keys.A);

            //Update the animation timer by the timeframe in milliseconds
            timer += (timeframe * 1000);
            shotTimer += (timeframe * 1000);
            if (Invulnerable)
            {
                invulnTimer += (timeframe * 1000);
                if (damageFlash)
                {
                    damageTimer += (timeframe * 1000);
                }

                if ((int)invulnTimer % 10 == 0 && damageTimer == 0)
                {
                    invulnFlash = !invulnFlash;
                }
            }

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

            if (invulnTimer >= invulnInterval)
            {
                invulnTimer = 0f;
                Invulnerable = false;
            }
            if (damageTimer >= damageFlashInterval)
            {
                damageTimer = 0f;
                damageFlash = false;
            }
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
                        else if (InventoryItem is Arrow)
                        {
                            if (currentState != BodyStates.WalkingShootLeft &&
                                currentState != BodyStates.WalkingShoot2Left &&
                                currentState != BodyStates.WalkingShoot3Left &&
                                currentState != BodyStates.WalkingDrawnLeft)
                            {
                                animations[BodyStates.WalkingShootLeft].CurrentFrame = 0;
                                currentState = BodyStates.WalkingShootLeft;
                            }
                        }

                        else if (currentState != BodyStates.WalkingLeft)
                        {
                            animations[BodyStates.WalkingLeft].CurrentFrame = 0;
                            currentState = BodyStates.WalkingStartLeft;
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

                        else if (InventoryItem is Arrow)
                        {
                            if (currentState != BodyStates.WalkingShootRight &&
                                currentState != BodyStates.WalkingShoot2Right &&
                                currentState != BodyStates.WalkingShoot3Right &&
                                currentState != BodyStates.WalkingDrawnRight)
                            {
                                animations[BodyStates.WalkingShootRight].CurrentFrame = 0;
                                currentState = BodyStates.WalkingShootRight;
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
                    Climbing = true;
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
            if (InputManager.Instance.Pressed(Keys.LeftControl))
            {
                isDucking = true;
                if (TouchingGround && !inHold)
                {
                    if (LeftFacingBodyState())
                    {
                        if (HoldingTorch == null && inventory.Count != 0 && inventory[activeItem] is Arrow)
                        {
                            currentState = BodyStates.DuckingLeftBow;
                        }
                        else
                            currentState = BodyStates.DuckingLeft;
                    }
                    else
                    {
                        if (HoldingTorch == null && inventory.Count != 0 && inventory[activeItem] is Arrow)
                        {
                            currentState = BodyStates.DuckingRightBow;
                        }
                        else
                            currentState = BodyStates.DuckingRight;
                    }
                }
            }
            else
                isDucking = false;
            #endregion

            #region Direction

            var up = InputManager.Instance.Pressed(Keys.Up);
            var down = InputManager.Instance.Pressed(Keys.Down);
            var right = InputManager.Instance.Pressed(Keys.Right);
            var left = InputManager.Instance.Pressed(Keys.Left);
            if (!ClimbingState() && !swinging && !VineBridgeState())
            {
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

                Logger.Debug("Jumped!");
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
                         /*   if (RightFacingBodyState())
                                currentState = BodyStates.ClimbingRight; //TODO -- Change to sideways climb state
                            else if (LeftFacingBodyState())
                                currentState = BodyStates.ClimbingLeft;*/


                            if (Physics.Position.X >= canClimb.Position.X) //We are to the right of the ladder
                            {
                                currentState = BodyStates.ClimbingLeft;
                                Physics.Position = new Vector2(CanClimb.Position.X + (PhysicsConstants.PixelsToMeters(CanClimb.Width) / 2) + 
                                                                                     (PhysicsConstants.PixelsToMeters(this.Width) / 2),
                                                               Physics.Position.Y);

                                WheelBody.Position = new Vector2(CanClimb.Position.X + (PhysicsConstants.PixelsToMeters(CanClimb.Width) / 2) +
                                                                                     (PhysicsConstants.PixelsToMeters(this.Width) / 2),
                                                               WheelBody.Position.Y);
                                movedirection.Y -= 1.0f;
                                Physics.LinearDamping = 5f;
                            }
                            else if (Physics.Position.X < canClimb.Position.X) //We are to the left of the ladder
                            {
                                currentState = BodyStates.ClimbingRight;
                                Physics.Position = new Vector2(CanClimb.Position.X - (PhysicsConstants.PixelsToMeters(CanClimb.Width) / 2) - 
                                                                                     (PhysicsConstants.PixelsToMeters(this.Width) / 2),
                                                               Physics.Position.Y);
                                WheelBody.Position = new Vector2(CanClimb.Position.X - (PhysicsConstants.PixelsToMeters(CanClimb.Width) / 2) -
                                                                                     (PhysicsConstants.PixelsToMeters(this.Width) / 2),
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
                if (HoldingTorch == null && inventory.Count != 0 && inventory[activeItem] is Arrow)
                {
                    if (facing == -1)
                        if (isDucking)
                        {
                            currentState = BodyStates.DuckShootLeftBow;
                        }
                        else
                            currentState = BodyStates.ShootingArrowLeft;
                    else

                        if (isDucking)
                        {
                            currentState = BodyStates.DuckShootRightBow;
                        }
                        else
                            currentState = BodyStates.ShootingArrowRight;
                    //currentState = BodyStates.ShootingRight;
                    holdTime = gameTime.TotalGameTime.TotalSeconds;
                    inHold = true;
                }
            }
            else if (!InputManager.Instance.Pressed(Keys.F) && inHold)
            {
                if (!ClimbingState() && !swinging && !VineBridgeState() &&
                    (shotTimer >= shotInterval) && HoldingTorch == null && inventory.Count != 0 && inventory[activeItem] is Arrow)
                {
                    inventory[activeItem].Use(this, world, gameTime, holdTime);
                    var shooting = animations[currentState].CurrentFrame = 0;
                    if (facing == -1)
                    {
                        if (isDucking)
                        {
                            currentState = BodyStates.DuckingLeftBow;
                        }
                        else
                            currentState = BodyStates.ShootingArrowNeutLeft;
                    }
                    else
                        if (isDucking)
                        {
                            currentState = BodyStates.DuckingRightBow;
                        }
                        else
                            currentState = BodyStates.ShootingArrowNeutRight;
                    shotTimer = 0f;
                }
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

            if (InputManager.Instance.IsNewKey(Keys.E))
            {
                if (onPickup != null)
                {
                    if (onPickup is Torch)
                    {
                        inventory.Add(onPickup);
                        ((Torch)onPickup).WeldToPlayer(this);
                        HoldingTorch = (Torch)onPickup;
                        onPickup = null;
                        EngineGame.Instance.LevelManager.RenderManager.UnregisterRenderable(currentItemPrompt);
                        
                    }
                }
                else if (onTorchGround != null && HoldingTorch != null)
                {
                    ((Torch)HoldingTorch).PlaceTorch(this, onTorchGround);
                    inventory.Remove(HoldingTorch);
                    activeItem = 0;
                    EngineGame.Instance.ScreenManager.CurrentGameplay.UpdatePrimaryItems(this);
                    HoldingTorch = null;

                }
            }
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
                    else if (currentState == BodyStates.RunningLeft)
                    {
                        currentState = BodyStates.RunningStopLeft;
                        timer = 0f;
                    }
                    else if (currentState == BodyStates.RunningRight)
                    {
                        currentState = BodyStates.RunningStopRight;
                        timer = 0f;
                    }
                    else if (currentState == BodyStates.WalkingShootLeft)
                    {
                        currentState = BodyStates.ShootingArrowNeutLeft; //Add in Stopping Animation
                        timer = 0f;
                    }
                    else if (currentState == BodyStates.WalkingShootRight)
                    {
                        currentState = BodyStates.ShootingArrowNeutRight; //Add in Stopping Animation
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
                if (currentState == BodyStates.ClimbingBack)
                {
                    animations[BodyStates.ClimbingBack].CurrentFrame = 0;
                    currentState = BodyStates.ClimbingBackNeut;
                }
                if (currentState == BodyStates.DuckingLeft)
                {
                    animations[BodyStates.DuckingLeft].CurrentFrame = 0;
                    currentState = BodyStates.NeutralLeft;
                }
                if (currentState == BodyStates.DuckingRight)
                {
                    currentState = BodyStates.NeutralRight;
                    animations[BodyStates.DuckingRight].CurrentFrame = 0;
                }
                if (currentState == BodyStates.DuckingLeftBow)
                {
                    animations[BodyStates.DuckingLeftBow].CurrentFrame = 0;
                    currentState = BodyStates.NeutralLeft;
                }
                if (currentState == BodyStates.DuckingRightBow)
                {
                    currentState = BodyStates.NeutralRight;
                    animations[BodyStates.DuckingRightBow].CurrentFrame = 0;
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
            else if (currentState == BodyStates.WalkingLeft && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingLeft];
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
                currentState = BodyStates.NeutralLeft;
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
            #region Running
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
            #endregion

            #region Shooting
            else if (currentState == BodyStates.ShootingArrowLeft && timer >= bowInterval)
            {
                var shooting = animations[BodyStates.ShootingArrowLeft];
                if (inHold)
                {
                    if (shooting.CurrentFrame != shooting.NumFrames - 1)
                        shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;
                }
                timer = 0f;
            }
            else if (currentState == BodyStates.ShootingArrowRight && timer >= bowInterval)
            {
                var shooting = animations[BodyStates.ShootingArrowRight];
                if (inHold)
                {
                    if (shooting.CurrentFrame != shooting.NumFrames - 1)
                        shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;
                }
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShootLeft && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingShoot2Left].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot2Left].NumFrames;
                    currentState = BodyStates.WalkingShoot2Left;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShoot2Left && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingShoot3Left].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot3Left].NumFrames;
                    currentState = BodyStates.WalkingShoot3Left;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShoot3Left && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingDrawnLeft].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot3Left].NumFrames;
                    currentState = BodyStates.WalkingDrawnLeft;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingDrawnLeft && timer >= interval)
            {
                var shooting = animations[currentState];
                shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }

            else if (currentState == BodyStates.WalkingShootRight && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingShoot2Right].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot2Right].NumFrames;
                    currentState = BodyStates.WalkingShoot2Right;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShoot2Right && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingShoot3Right].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot3Right].NumFrames;
                    currentState = BodyStates.WalkingShoot3Right;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShoot3Right && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingDrawnRight].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot3Right].NumFrames;
                    currentState = BodyStates.WalkingDrawnRight;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingDrawnRight && timer >= interval)
            {
                var shooting = animations[currentState];
                shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.DuckShootLeftBow && timer >= bowInterval)
            {
                var shooting = animations[BodyStates.DuckShootLeftBow];
                if (inHold)
                {
                    if (shooting.CurrentFrame != shooting.NumFrames - 1)
                        shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;
                }
                timer = 0f;
            }
            else if (currentState == BodyStates.DuckShootRightBow && timer >= bowInterval)
            {
                var shooting = animations[BodyStates.DuckShootRightBow];
                if (inHold)
                {
                    if (shooting.CurrentFrame != shooting.NumFrames - 1)
                        shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;
                }
                timer = 0f;
            }
            #endregion

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
            if (currentState == BodyStates.ClimbingBack && timer >= interval)
            {
                var climbing = animations[BodyStates.ClimbingBack];

                    climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                 timer = 0f;
            }
            if (currentState == BodyStates.KnockbackRight && timer >= interval)
            {
                var knockback = animations[BodyStates.KnockbackRight];
                if (knockback.CurrentFrame == knockback.NumFrames - 1)
                {
                    knockback.CurrentFrame = 0;
                    currentState = BodyStates.NeutralRight;
                }
                else
                {
                    knockback.CurrentFrame = (knockback.CurrentFrame + 1) % knockback.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.KnockbackLeft && timer >= interval)
            {
                var knockback = animations[BodyStates.KnockbackLeft];
                if (knockback.CurrentFrame == knockback.NumFrames - 1)
                {
                    knockback.CurrentFrame = 0;
                    currentState = BodyStates.NeutralLeft;
                }
                else
                {
                    knockback.CurrentFrame = (knockback.CurrentFrame + 1) % knockback.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.DuckingLeft && timer >= interval)
            {
                var ducking = animations[BodyStates.DuckingLeft];
                if (ducking.CurrentFrame != ducking.NumFrames - 2)
                {
                    ducking.CurrentFrame = (ducking.CurrentFrame + 1) % ducking.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.DuckingRight && timer >= interval)
            {
                var ducking = animations[BodyStates.DuckingRight];
                if (ducking.CurrentFrame != ducking.NumFrames - 2)
                {
                    ducking.CurrentFrame = (ducking.CurrentFrame + 1) % ducking.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.DuckingLeftBow && timer >= interval)
            {
                var ducking = animations[BodyStates.DuckingLeftBow];
                if (ducking.CurrentFrame != ducking.NumFrames - 1)
                {
                    ducking.CurrentFrame = (ducking.CurrentFrame + 1) % ducking.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.DuckingRightBow && timer >= interval)
            {
                var ducking = animations[BodyStates.DuckingRightBow];
                if (ducking.CurrentFrame != ducking.NumFrames - 1)
                {
                    ducking.CurrentFrame = (ducking.CurrentFrame + 1) % ducking.NumFrames;
                }
                timer = 0f;
            }
        }

        //bool OnCollidedWith(Fixture f, WorldGeometry2 world, Fixture wf, Contact info)
        //{
        //    Vector2 normal;
        //    FixedArray2<Vector2> points;
        //    info.GetWorldManifold(out normal, out points);

        //    return true;
        //}

        bool OnCollidedWith(Fixture f, TorchGround torchGround, Fixture c, Contact info)
        {
            if (HoldingTorch != null)
            {
                currentItemPrompt = new ItemPopup("Textures/Keys/e-Key",
                                                    torchGround.Physics.Position);

                EngineGame.Instance.LevelManager.RenderManager.RegisterRenderable(currentItemPrompt);
            }

            onTorchGround = torchGround;
            return true;
        }
        void OnSeparation(Fixture f1, TorchGround torchGround, Fixture f2)
        {
            EngineGame.Instance.LevelManager.RenderManager.UnregisterRenderable(currentItemPrompt);
            onTorchGround = null;
        }

        bool OnCollidedWith(Fixture f, WorldGeometry2 wg, Fixture c, Contact info)
        {
            if (c.UserData is OneWayPlatform && climbing)
                return false;

            else
                return info.Enabled;

        }

        bool OnCollidedWith(Fixture f, Torch torch, Fixture c, Contact info)
        {
            OnPickup = torch;
            currentItemPrompt = new ItemPopup("Textures/Keys/e-Key", torch.Physics.Position - 
                                              new Vector2(0, PhysicsConstants.PixelsToMeters(torch.Height) / 2));

            EngineGame.Instance.LevelManager.RenderManager.RegisterRenderable(currentItemPrompt);
            
            return true;
        }
        void OnSeparation(Fixture f1, Torch torch, Fixture f2)
        {
            OnPickup = null;
            EngineGame.Instance.LevelManager.RenderManager.UnregisterRenderable(currentItemPrompt);
        }

        private VineBridge vineBridge;
        bool OnCollidedWith(Fixture f, VineBridge bridge, Fixture vbf, Contact info)
        {
            if (LeftFacingBodyState())
                currentState = BodyStates.HorizontalClimbLeft;
            else
                currentState = BodyStates.HorizontalClimbRight;

            vineBridge = bridge;

            return true;
        }

        void OnSeparation(Fixture f1, VineBridge bridge, Fixture f2)
        {
            if (LeftFacingBodyState())
                currentState = BodyStates.JumpingLeft;
            else
                currentState = BodyStates.JumpingRight;
        }

        public bool OnCollidedWith(Fixture f1, Bramble bramble, Fixture f2, Contact info)
        {

            if (!Invulnerable)
            {
                this.RegisterDot(bramble.dot);
                bramble.dot.Active = true;
                return true;
            }
            else
                return false;
        }

        public void OnSeparation(Fixture f1, Bramble bramble, Fixture f2)
        {
            bramble.dot.Active = false;
        }

        private WeldJoint vineJoint;
        private bool swinging;
        private bool leftVine = true;
        bool OnCollidedWith(Fixture f, Vine vine, Fixture vf, Contact info)
        {
            if (!swinging && leftVine)
            {
                var pointOnVine = new Vector2(0, PhysicsConstants.PixelsToMeters((int)(vine.Height * .2)));

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

        void OnSeparation(Fixture f1, Vine vine, Fixture f2)
        {
            leftVine = true;
        }

        private void ForceVineSeperate()
        {
            Physics.AngularVelocity = 0;
            Physics.Rotation = 0;
            Physics.FixedRotation = true;
            _world.RemoveJoint(vineJoint);
            swinging = false;
        }

        public override IRendering Rendering
        {
            get
            {
                var anim = animations[currentState];
                if (invulnFlash)
                {
                    anim.UpdateTint(new Color(0, 0, 0, 0));
                }
                else if(damageFlash)
                {
                    anim.UpdateTint(new Color(255f, 0, 0, .5f));
                }
                else
                    anim.UpdateTint(Color.White);
                anim.Position = PhysicsConstants.MetersToPixels(Physics.Position);
                anim.Rotation = Physics.Rotation;
                return anim;
            }
        }

        private Dictionary<BodyStates, NewAnimationRendering> CreateAnimations()
        {
            var dictionary = new Dictionary<BodyStates, NewAnimationRendering>();
            Color invulnTint = Color.White;
            #region Neutral

            dictionary.Add(
                BodyStates.NeutralRight,
                new NewAnimationRendering(
                    NEUTRAL_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.NeutralLeft,
                 new NewAnimationRendering(
                    NEUTRAL_LEFT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
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
                        Vector2.One,
                    invulnTint));
            dictionary.Add(
                BodyStates.IdleRightClosed,
                new NewAnimationRendering(
                        IDLE_CLOSED_HAND,
                        new Vector2(76.8f, 153.6f),
                        5,
                        Vector2.Zero,
                        0,
                        Vector2.One,
                    invulnTint));

            #endregion

            #region Walking

            dictionary.Add(BodyStates.WalkingStartRight,
                new NewAnimationRendering(
                    WALKING_RIGHT_INTERMEDIATE,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.WalkingRight,
                new NewAnimationRendering(
                    WALKING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.WalkingEndRight,
                new NewAnimationRendering(
                    WALKING_RIGHT_INTERMEDIATE,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.WalkingStartLeft,
                new NewAnimationRendering(
                    WALKING_LEFT_INTERMEDIATE,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.WalkingLeft,
                new NewAnimationRendering(
                    WALKING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.WalkingEndLeft,
                new NewAnimationRendering(
                    WALKING_LEFT_INTERMEDIATE,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            #endregion

            #region Running
            dictionary.Add(BodyStates.RunningStopLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT_INTERMEDIATE,
                    new Vector2(153.6f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.RunningStopRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT_INTERMEDIATE,
                    new Vector2(153.6f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.RunningStartLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT_INTERMEDIATE,
                    new Vector2(153.6f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.RunningStartRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT_INTERMEDIATE,
                    new Vector2(153.6f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.RunningLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT,
                    new Vector2(153.6f, 153.6f),
                    8,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.RunningRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT,
                    new Vector2(153.6f, 153.6f),
                    8,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            #endregion
            #region Jumping

            dictionary.Add(BodyStates.JumpingRight,
                new NewAnimationRendering(
                    JUMPING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.JumpingLeft,
                new NewAnimationRendering(
                    JUMPING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            #endregion

            #region Climbing
            dictionary.Add(BodyStates.ClimbingBack,
                new NewAnimationRendering(
                    CLIMBING_BACK,
                    new Vector2(76.8f, 153.6f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.ClimbingBackNeut,
                new NewAnimationRendering(
                    CLIMBING_NEUT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.ClimbingLeft,
               new NewAnimationRendering(
                    CLIMBING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.ClimbingRight,
               new NewAnimationRendering(
                    CLIMBING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.ClimbingRightNeutral,
               new NewAnimationRendering(
                    CLIMBING_NEUTRAL_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.ClimbingLeftNeutral,
               new NewAnimationRendering(
                    CLIMBING_NEUTRAL_LEFT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.ClimbingLookRight,
               new NewAnimationRendering(
                    CLIMBING_LOOKING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.ClimbingLookLeft,
               new NewAnimationRendering(
                    CLIMBING_LOOKING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.HorizontalClimbLeft,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_LEFT,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.HorizontalClimbRight,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            dictionary.Add(BodyStates.HorizontalClimbRightNeut,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_RIGHT_NEUT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(BodyStates.HorizontalClimbLeftNeut,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_LEFT_NEUT,
                    new Vector2(76.8f, 153.6f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            #endregion

            #region Shooting

            dictionary.Add(
                BodyStates.ShootingArrowLeft,
                 new NewAnimationRendering(
                    SHOOT_ARROW_LEFT,
                    new Vector2(153.6f, 185f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.ShootingArrowRight,
                 new NewAnimationRendering(
                    SHOOT_ARROW_RIGHT,
                    new Vector2(153.6f, 185f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            #endregion
            #region WalkingShooting

            dictionary.Add(
                BodyStates.WalkingDrawnLeft,
                 new NewAnimationRendering(
                    WALK_DRAWN_LEFT,
                    new Vector2(94f, 169f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.WalkingDrawnRight,
                 new NewAnimationRendering(
                    WALK_DRAWN_RIGHT,
                    new Vector2(94f, 171f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.WalkingShootLeft,
                 new NewAnimationRendering(
                    WALK_SHOOT_LEFT,
                    new Vector2(158f, 146f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.WalkingShootRight,
                 new NewAnimationRendering(
                    WALK_SHOOT_RIGHT,
                    new Vector2(159f, 146f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.WalkingShoot2Left,
                 new NewAnimationRendering(
                    WALK_SHOOT2_LEFT,
                    new Vector2(135f, 147f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.WalkingShoot2Right,
                 new NewAnimationRendering(
                    WALK_SHOOT2_RIGHT,
                    new Vector2(135f, 147f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.WalkingShoot3Left,
                 new NewAnimationRendering(
                    WALK_SHOOT3_LEFT,
                    new Vector2(104f, 147f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.WalkingShoot3Right,
                 new NewAnimationRendering(
                    WALK_SHOOT3_RIGHT,
                    new Vector2(104f, 147f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.ShootingArrowNeutLeft,
                 new NewAnimationRendering(
                    SHOOT_NEUT_LEFT,
                    new Vector2(158f, 146f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.ShootingArrowNeutRight,
                 new NewAnimationRendering(
                    SHOOT_NEUT_RIGHT,
                    new Vector2(159f, 146f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            #endregion

            #region Knockback
            dictionary.Add(
                BodyStates.KnockbackRight,
                new NewAnimationRendering(
                    KNOCKBACK_RIGHT,
                    new Vector2(76.8f, 153.6f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.KnockbackLeft,
                new NewAnimationRendering(
                    KNOCKBACK_LEFT,
                    new Vector2(76.8f, 153.6f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));
            #endregion

            #region Ducking
            dictionary.Add(
                BodyStates.DuckingLeft,
                 new NewAnimationRendering(
                    DUCK_LEFT,
                    new Vector2(154f, 154f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.DuckingRight,
                 new NewAnimationRendering(
                    DUCK_RIGHT,
                    new Vector2(154f, 154f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.DuckingLeftBow,
                 new NewAnimationRendering(
                    DUCK_LEFT_BOW,
                    new Vector2(154f, 154f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.DuckShootLeftBow,
                 new NewAnimationRendering(
                    DUCK_LEFT_SHOOT_BOW,
                    new Vector2(154f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.DuckingRightBow,
                 new NewAnimationRendering(
                    DUCK_RIGHT_BOW,
                    new Vector2(154f, 154f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

            dictionary.Add(
                BodyStates.DuckShootRightBow,
                 new NewAnimationRendering(
                    DUCK_RIGHT_SHOOT_BOW,
                    new Vector2(154f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint));

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
                    currentState == BodyStates.WalkingDrawnRight ||
                    currentState == BodyStates.WalkingShootRight ||
                    currentState == BodyStates.WalkingShoot2Right ||
                    currentState == BodyStates.WalkingShoot3Right ||
                    currentState == BodyStates.ShootingArrowRight ||
                    currentState == BodyStates.ShootingArrowNeutRight ||
                    currentState == BodyStates.RunningRight ||
                    currentState == BodyStates.NeutralRight ||
                    currentState == BodyStates.IdleRightOpen ||
                    currentState == BodyStates.IdleRightClosed ||
                    currentState == BodyStates.DuckingRight ||
                    currentState == BodyStates.DuckingRightBow ||
                    currentState == BodyStates.DuckShootRightBow ||
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
                    currentState == BodyStates.WalkingDrawnLeft ||
                    currentState == BodyStates.WalkingShootLeft ||
                    currentState == BodyStates.WalkingShoot2Left ||
                    currentState == BodyStates.WalkingShoot3Left ||
                    currentState == BodyStates.ShootingArrowLeft ||
                    currentState == BodyStates.ShootingArrowNeutLeft||
                    currentState == BodyStates.RunningLeft ||
                    currentState == BodyStates.NeutralLeft ||
                    currentState == BodyStates.IdleLeftOpen ||
                    currentState == BodyStates.IdleLeftClosed ||
                    currentState == BodyStates.DuckingLeft ||
                    currentState == BodyStates.DuckingLeftBow ||
                    currentState == BodyStates.DuckShootLeftBow ||
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
        public bool VineBridgeState()
        {
            return (currentState == BodyStates.HorizontalClimbLeft ||
                    currentState == BodyStates.HorizontalClimbRight ||
                    currentState == BodyStates.HorizontalClimbLeftNeut ||
                    currentState == BodyStates.HorizontalClimbRightNeut);
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
            Dots.Add(dot);
        }

        private bool initialized;
        private RevoluteJoint MotorJoint;
        public float RopeAttachHeight;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                _world = world;

                Width = spriteWidth;
                Height = spriteHeight;
                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                Physics = BodyFactory.CreateBody(world, Position, this);

                var wPos = Position + new Vector2(0, (spriteHeightMeters - spriteWidthMeters) / 2);
                WheelBody = BodyFactory.CreateBody(world, wPos, this);

                var r = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters - spriteWidthMeters / 2,
                    1.4f,
                    new Vector2(0, -spriteWidthMeters / 4),
                    Physics);
                
                var c = FixtureFactory.AttachCircle(
                    spriteWidthMeters / 2,
                    1.4f,
                    WheelBody);

                r.CollidesWith = Category.Cat1 | ~Category.Cat31;
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
                Physics.IsBullet = true;
                
                RopeAttachHeight = -4 * (PhysicsConstants.PixelsToMeters(Height) / 9);

                var ropeSensor = FixtureFactory.AttachCircle(
                    .08f, 5, Physics, new Vector2(0, RopeAttachHeight));
                ropeSensor.Friction = 5f;
                ropeSensor.Restitution = 1f;
                ropeSensor.UserData = this;
                ropeSensor.IsSensor = true;
                ropeSensor.CollidesWith = Category.Cat4;
                ropeSensor.CollisionCategories = Category.Cat4;

                ropeSensor.RegisterOnCollidedListener<VineBridge>(OnCollidedWith);
                ropeSensor.RegisterOnSeparatedListener<VineBridge>(OnSeparation);

                var vineSensor = FixtureFactory.AttachCircle(.1f, 5, Physics, Vector2.Zero);
                vineSensor.Friction = 5f;
                vineSensor.Restitution = 1f;
                vineSensor.UserData = this;
                vineSensor.IsSensor = true;
                vineSensor.CollidesWith = Category.Cat5;
                vineSensor.CollisionCategories = Category.Cat5;

                vineSensor.RegisterOnCollidedListener<Vine>(OnCollidedWith);
                vineSensor.RegisterOnSeparatedListener<Vine>(OnSeparation);

                Physics.RegisterOnCollidedListener<Bramble>(OnCollidedWith);
                Physics.RegisterOnSeparatedListener<Bramble>(OnSeparation);
                r.RegisterOnCollidedListener<Torch>(OnCollidedWith);
                r.RegisterOnSeparatedListener<Torch>(OnSeparation);
                c.RegisterOnCollidedListener<TorchGround>(OnCollidedWith);
                c.RegisterOnSeparatedListener<TorchGround>(OnSeparation);
                c.RegisterOnCollidedListener<WorldGeometry2>(OnCollidedWith);

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
            WheelBody.Dispose();
        }

        public void Reset(Vector2 newPos, IComponentContext engineRegistrations)
        {
            //Physics.Dispose();
            //WheelBody.Dispose();
            //InitializePhysics(true, engineRegistrations);

            //Physics.Position = newPos;
        }

        private void LogMetricSnapshot()
        {
            Logger.Info(String.Format("Player health: {0}", Health));
            Logger.Info(String.Format("Player mana: {0}", Mana));
            Logger.Info(String.Format("Player positon: {0}", Position));
        }
    }
}